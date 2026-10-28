using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PokerPlanning.Rooms;

namespace PokerPlanning.Hubs
{
    public class PokerHub : Hub
    {
        private readonly RoomStore _rooms;

        public PokerHub(RoomStore rooms)
        {
            _rooms = rooms;
        }

        public async Task JoinRoom(string roomId, string name, bool isObserver)
        {
            var id = RoomStore.NormalizeRoomId(roomId);
            if (id.Length == 0)
                throw new HubException("Room name must contain letters or numbers.");

            // Re-joining the same room (e.g. to change name) keeps the room and its story intact.
            if (_rooms.FindByConnection(Context.ConnectionId)?.Id != id)
                await LeaveCurrentRoom();
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            await Broadcast(_rooms.Join(Context.ConnectionId, id, name, isObserver));
        }

        public Task Vote(string card) => Mutate(room => room.Vote(Context.ConnectionId, card));

        public Task Reveal() => Mutate(room => room.Reveal());

        public Task Reset() => Mutate(room => room.Reset());

        public Task SetStory(string story) => Mutate(room => room.SetStory(story));

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await LeaveCurrentRoom();
            await base.OnDisconnectedAsync(exception);
        }

        private async Task Mutate(Func<Room, RoomState> action)
        {
            var room = _rooms.FindByConnection(Context.ConnectionId)
                       ?? throw new HubException("Join a room first.");

            RoomState state;
            try
            {
                state = action(room);
            }
            catch (Exception e) when (e is InvalidOperationException or ArgumentException)
            {
                throw new HubException(e.Message);
            }

            await Broadcast(state);
        }

        private async Task LeaveCurrentRoom()
        {
            var state = _rooms.Leave(Context.ConnectionId);
            if (state is null)
                return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, state.RoomId);
            await Broadcast(state);
        }

        private Task Broadcast(RoomState state) => Clients.Group(state.RoomId).SendAsync("roomState", state);
    }
}
