using System.Collections.Immutable;

namespace PokerPlanning.Rooms
{
    public record RoomState(
        string RoomId,
        string Story,
        bool Revealed,
        ImmutableList<string> Deck,
        ImmutableList<ParticipantState> Participants,
        VoteSummary Summary);

    /// <summary>
    /// Vote is only populated once the room has been revealed, so nobody can peek at other votes.
    /// </summary>
    public record ParticipantState(
        string ConnectionId,
        string Name,
        bool IsObserver,
        bool HasVoted,
        string Vote);

    public record VoteSummary(
        int VoteCount,
        double? Average,
        bool Consensus,
        ImmutableList<CardCount> Distribution);

    public record CardCount(string Card, int Count);
}
