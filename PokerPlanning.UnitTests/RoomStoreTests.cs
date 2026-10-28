using FluentAssertions;
using PokerPlanning.Rooms;
using Xunit;

namespace PokerPlanning.UnitTests
{
    public class RoomStoreTests
    {
        private readonly RoomStore _sut = new();

        [Theory]
        [InlineData("Team Rocket", "team-rocket")]
        [InlineData("  sprint 42!  ", "sprint-42")]
        [InlineData("!!!", "")]
        public void Room_Ids_Are_Url_Friendly(string input, string expected)
        {
            RoomStore.NormalizeRoomId(input).Should().Be(expected);
        }

        [Fact]
        public void Connections_Are_Tracked_To_Their_Room()
        {
            _sut.Join("a", "team", "Ali", false);

            _sut.FindByConnection("a").Id.Should().Be("team");
            _sut.FindByConnection("unknown").Should().BeNull();
        }

        [Fact]
        public void Room_Is_Removed_When_Last_Participant_Leaves()
        {
            _sut.Join("a", "team", "Ali", false);
            _sut.Join("b", "team", "Sam", false);

            _sut.Leave("a").Participants.Should().ContainSingle(p => p.Name == "Sam");
            _sut.Leave("b").Participants.Should().BeEmpty();

            _sut.FindByConnection("b").Should().BeNull();
            _sut.Join("c", "team", "Kim", false).Participants.Should().ContainSingle();
        }

        [Fact]
        public void Leaving_Without_A_Room_Returns_Null()
        {
            _sut.Leave("ghost").Should().BeNull();
        }
    }
}
