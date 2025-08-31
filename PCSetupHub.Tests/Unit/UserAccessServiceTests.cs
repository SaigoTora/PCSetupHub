using Moq;

using PCSetupHub.Core.Services;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Tests.Unit
{
	public class UserAccessServiceTests
	{
		#region Tests

		#region Friends
		[Fact]
		public async Task HasAccessAsync_RequesterIsFriend_LevelIsFriendsOnly_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Accepted);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsOnly;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasAccessAsync_RequesterIsFriend_LevelIsFriendsAndFollowings_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Accepted);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsAndFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasAccessAsync_RequesterIsFriend_LevelIsFriendsFollowersFollowings_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Accepted);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsFollowersFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasAccessAsync_RequesterIsFriend_LevelIsEveryone_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Accepted);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.Everyone;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}
		#endregion

		#region Followings
		[Fact]
		public async Task HasAccessAsync_TargetUserIsFollower_LevelIsFriendsOnly_False()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(targetUser, requester, FriendshipStatusType.Pending);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsOnly;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task HasAccessAsync_TargetUserIsFollower_LevelIsFriendsAndFollowings_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(targetUser, requester, FriendshipStatusType.Cancelled);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsAndFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasAccessAsync_TargetUserIsFollower_LevelIsFriendsFollowersFollowings_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(targetUser, requester, FriendshipStatusType.Pending);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsFollowersFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasAccessAsync_TargetUserIsFollower_LevelIsEveryone_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(targetUser, requester, FriendshipStatusType.Cancelled);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.Everyone;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}
		#endregion

		#region Followers
		[Fact]
		public async Task HasAccessAsync_RequesterIsFollower_LevelIsFriendsOnly_False()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Pending);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsOnly;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task HasAccessAsync_RequesterIsFollower_LevelIsFriendsAndFollowings_False()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Cancelled);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsAndFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task HasAccessAsync_RequesterIsFollower_LevelIsFriendsFollowersFollowings_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Pending);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsFollowersFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task HasAccessAsync_RequesterIsFollower_LevelIsEveryone_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();
			SetupFriendship(requester, targetUser, FriendshipStatusType.Cancelled);

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.Everyone;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}
		#endregion

		#region No friendship
		[Fact]
		public async Task HasAccessAsync_NoFriendship_LevelIsFriendsOnly_False()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsOnly;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.False(result);
		}
		[Fact]
		public async Task HasAccessAsync_NoFriendship_LevelIsFriendsAndFollowings_False()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsAndFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.False(result);
		}
		[Fact]
		public async Task HasAccessAsync_NoFriendship_LevelIsFriendsFollowersFollowings_False()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.FriendsFollowersFollowings;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.False(result);
		}
		[Fact]
		public async Task HasAccessAsync_NoFriendship_LevelIsEveryone_True()
		{
			// Arrange
			User requester = CreateRequester();
			User targetUser = CreateTargetUser();

			Mock<IUserRepository> mockRepo = new();
			SetupUserByLogin(mockRepo, targetUser);
			UserAccessService service = new(mockRepo.Object);
			PrivacyLevelType privacyLevel = PrivacyLevelType.Everyone;

			// Act
			bool result = await service.HasAccessAsync(requester.Login, targetUser.Login,
				privacyLevel);

			// Assert
			Assert.True(result);
		}

		#endregion

		#endregion

		#region Helpers
		private static User CreateRequester()
		{
			User requester = new() { Login = "requester" };
			requester.SetId(1);

			return requester;
		}
		private static User CreateTargetUser()
		{
			User targetUser = new() { Login = "targetUser" };
			targetUser.SetId(2);

			return targetUser;
		}
		private static void SetupUserByLogin(Mock<IUserRepository> mockRepo, User user)
		{
			const UserIncludes USER_INCLUDES
				= UserIncludes.PrivacySetting | UserIncludes.Friendships;
			const bool AS_NO_TRACKING = true;

			mockRepo
				.Setup(r => r.GetByLoginAsync(user.Login, USER_INCLUDES, AS_NO_TRACKING))
				.ReturnsAsync(user);
		}
		private static void SetupFriendship(User requester, User targetUser,
			FriendshipStatusType status)
		{
			Friendship friendship = new(requester.Id, targetUser.Id, (int)status);
			friendship.SetUsers(requester, targetUser);

			requester.SentFriendRequests = [
				friendship
				];
			targetUser.ReceivedFriendRequests = [
				friendship
				];
		}
		#endregion
	}
}