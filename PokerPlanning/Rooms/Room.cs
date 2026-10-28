using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace PokerPlanning.Rooms
{
    /// <summary>
    /// A single planning poker table. All mutations are serialized through a lock,
    /// because SignalR invokes hub methods for different connections concurrently.
    /// </summary>
    public sealed class Room
    {
        public static readonly ImmutableList<string> Deck =
            ImmutableList.Create("0", "1", "2", "3", "5", "8", "13", "21", "?", "☕");

        private const int MaxNameLength = 30;
        private const int MaxStoryLength = 200;

        private readonly object _gate = new();
        private readonly Dictionary<string, Participant> _participants = new();
        private string _story = string.Empty;
        private bool _revealed;

        public Room(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public bool IsEmpty
        {
            get
            {
                lock (_gate)
                {
                    return _participants.Count == 0;
                }
            }
        }

        public RoomState Join(string connectionId, string name, bool isObserver)
        {
            lock (_gate)
            {
                _participants[connectionId] = new Participant(Clean(name, MaxNameLength, "Anonymous"), isObserver);
                return Snapshot();
            }
        }

        public RoomState Leave(string connectionId)
        {
            lock (_gate)
            {
                _participants.Remove(connectionId);
                return Snapshot();
            }
        }

        public RoomState Vote(string connectionId, string card)
        {
            lock (_gate)
            {
                if (!_participants.TryGetValue(connectionId, out var participant))
                    throw new InvalidOperationException("Join the room before voting.");
                if (participant.IsObserver)
                    throw new InvalidOperationException("Observers cannot vote.");
                if (_revealed)
                    throw new InvalidOperationException("Votes are already revealed. Start a new round to vote again.");

                // Picking the same card again withdraws the vote.
                if (card is null || card == participant.Vote)
                    participant.Vote = null;
                else if (Deck.Contains(card))
                    participant.Vote = card;
                else
                    throw new ArgumentException($"'{card}' is not a card in the deck.", nameof(card));

                return Snapshot();
            }
        }

        public RoomState Reveal()
        {
            lock (_gate)
            {
                _revealed = true;
                return Snapshot();
            }
        }

        public RoomState Reset()
        {
            lock (_gate)
            {
                ClearVotes();
                return Snapshot();
            }
        }

        public RoomState SetStory(string story)
        {
            lock (_gate)
            {
                _story = Clean(story, MaxStoryLength, string.Empty);
                ClearVotes();
                return Snapshot();
            }
        }

        public RoomState GetState()
        {
            lock (_gate)
            {
                return Snapshot();
            }
        }

        private void ClearVotes()
        {
            _revealed = false;
            foreach (var participant in _participants.Values)
                participant.Vote = null;
        }

        private RoomState Snapshot()
        {
            var participants = _participants
                .Select(p => new ParticipantState(
                    p.Key,
                    p.Value.Name,
                    p.Value.IsObserver,
                    p.Value.Vote is not null,
                    _revealed ? p.Value.Vote : null))
                .OrderBy(p => p.IsObserver)
                .ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToImmutableList();

            return new RoomState(Id, _story, _revealed, Deck, participants, _revealed ? Summarize() : null);
        }

        private VoteSummary Summarize()
        {
            var votes = _participants.Values
                .Select(p => p.Vote)
                .Where(v => v is not null)
                .ToList();

            var numeric = votes
                .Select(v => double.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out var n) ? n : (double?)null)
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .ToList();

            var distribution = votes
                .GroupBy(v => v)
                .OrderBy(g => Deck.IndexOf(g.Key))
                .Select(g => new CardCount(g.Key, g.Count()))
                .ToImmutableList();

            return new VoteSummary(
                votes.Count,
                numeric.Count > 0 ? Math.Round(numeric.Average(), 1) : null,
                distribution.Count == 1,
                distribution);
        }

        private static string Clean(string value, int maxLength, string fallback)
        {
            var trimmed = value?.Trim() ?? string.Empty;
            if (trimmed.Length == 0)
                return fallback;
            return trimmed.Length > maxLength ? trimmed[..maxLength] : trimmed;
        }

        private sealed class Participant
        {
            public Participant(string name, bool isObserver)
            {
                Name = name;
                IsObserver = isObserver;
            }

            public string Name { get; }
            public bool IsObserver { get; }
            public string Vote { get; set; }
        }
    }
}
