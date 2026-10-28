using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace PokerPlanning.Rooms
{
    /// <summary>
    /// Keeps rooms in memory and remembers which room each connection sits in.
    /// Rooms disappear once the last participant leaves.
    /// </summary>
    public sealed class RoomStore
    {
        private static readonly Regex InvalidRoomIdChars = new("[^a-z0-9-]", RegexOptions.Compiled);
        private const int MaxRoomIdLength = 40;

        private readonly ConcurrentDictionary<string, Room> _rooms = new();
        private readonly ConcurrentDictionary<string, string> _roomByConnection = new();
        private readonly object _gate = new();

        public static string NormalizeRoomId(string roomId)
        {
            var id = InvalidRoomIdChars.Replace((roomId ?? string.Empty).Trim().ToLowerInvariant(), "-").Trim('-');
            return id.Length > MaxRoomIdLength ? id[..MaxRoomIdLength] : id;
        }

        public RoomState Join(string connectionId, string roomId, string name, bool isObserver)
        {
            lock (_gate)
            {
                var room = _rooms.GetOrAdd(roomId, id => new Room(id));
                _roomByConnection[connectionId] = roomId;
                return room.Join(connectionId, name, isObserver);
            }
        }

        /// <summary>
        /// Removes the connection from its room. Returns the room's new state, or null if it was not in one.
        /// </summary>
        public RoomState Leave(string connectionId)
        {
            lock (_gate)
            {
                if (!_roomByConnection.TryRemove(connectionId, out var roomId) || !_rooms.TryGetValue(roomId, out var room))
                    return null;

                var state = room.Leave(connectionId);
                if (room.IsEmpty)
                    _rooms.TryRemove(roomId, out _);
                return state;
            }
        }

        public Room FindByConnection(string connectionId)
        {
            return _roomByConnection.TryGetValue(connectionId, out var roomId) && _rooms.TryGetValue(roomId, out var room)
                ? room
                : null;
        }
    }
}
