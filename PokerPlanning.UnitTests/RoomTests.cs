using System;
using System.Linq;
using FluentAssertions;
using PokerPlanning.Rooms;
using Xunit;

namespace PokerPlanning.UnitTests
{
    public class RoomTests
    {
        private readonly Room _sut = new("refinement");

        [Fact]
        public void Votes_Are_Hidden_Until_Revealed()
        {
            // Arrange
            _sut.Join("a", "Ali", isObserver: false);
            _sut.Join("b", "Sam", isObserver: false);

            // Act
            var state = _sut.Vote("a", "5");

            // Assert
            state.Revealed.Should().BeFalse();
            state.Summary.Should().BeNull();
            state.Participants.Single(p => p.Name == "Ali").HasVoted.Should().BeTrue();
            state.Participants.Should().OnlyContain(p => p.Vote == null);
        }

        [Fact]
        public void Reveal_Shows_Votes_And_Summary()
        {
            // Arrange
            _sut.Join("a", "Ali", false);
            _sut.Join("b", "Sam", false);
            _sut.Join("c", "Kim", false);
            _sut.Vote("a", "3");
            _sut.Vote("b", "8");
            _sut.Vote("c", "?");

            // Act
            var state = _sut.Reveal();

            // Assert
            state.Participants.Single(p => p.Name == "Sam").Vote.Should().Be("8");
            state.Summary.VoteCount.Should().Be(3);
            state.Summary.Average.Should().Be(5.5);
            state.Summary.Consensus.Should().BeFalse();
            state.Summary.Distribution.Select(d => d.Card).Should().Equal("3", "8", "?");
        }

        [Fact]
        public void Matching_Votes_Are_A_Consensus()
        {
            _sut.Join("a", "Ali", false);
            _sut.Join("b", "Sam", false);
            _sut.Vote("a", "13");
            _sut.Vote("b", "13");

            var state = _sut.Reveal();

            state.Summary.Consensus.Should().BeTrue();
            state.Summary.Average.Should().Be(13);
        }

        [Fact]
        public void Picking_The_Same_Card_Again_Withdraws_The_Vote()
        {
            _sut.Join("a", "Ali", false);
            _sut.Vote("a", "5");

            var state = _sut.Vote("a", "5");

            state.Participants.Single().HasVoted.Should().BeFalse();
        }

        [Fact]
        public void Reset_Clears_Votes_And_Hides_Them_Again()
        {
            _sut.Join("a", "Ali", false);
            _sut.Vote("a", "5");
            _sut.Reveal();

            var state = _sut.Reset();

            state.Revealed.Should().BeFalse();
            state.Participants.Single().HasVoted.Should().BeFalse();
        }

        [Fact]
        public void Changing_The_Story_Starts_A_New_Round()
        {
            _sut.Join("a", "Ali", false);
            _sut.Vote("a", "5");
            _sut.Reveal();

            var state = _sut.SetStory("  PROJ-42 Export button on Safari  ");

            state.Story.Should().Be("PROJ-42 Export button on Safari");
            state.Revealed.Should().BeFalse();
            state.Participants.Single().HasVoted.Should().BeFalse();
        }

        [Fact]
        public void Observers_Cannot_Vote()
        {
            _sut.Join("a", "Product Owner", isObserver: true);

            Action act = () => _sut.Vote("a", "5");

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Cannot_Vote_After_Reveal()
        {
            _sut.Join("a", "Ali", false);
            _sut.Reveal();

            Action act = () => _sut.Vote("a", "5");

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Unknown_Cards_Are_Rejected()
        {
            _sut.Join("a", "Ali", false);

            Action act = () => _sut.Vote("a", "100");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Blank_Names_Become_Anonymous()
        {
            var state = _sut.Join("a", "   ", false);

            state.Participants.Single().Name.Should().Be("Anonymous");
        }
    }
}
