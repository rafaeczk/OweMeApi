using Domain.Common;

namespace Domain.Exceptions;

public class FriendCodeExpiredException() : DomainException("Friend code expired");

public class YourFriendCodeException() : DomainException("You cannot enter your own code");

public class SelfFriendshipOperationException() : DomainException("You cannot operate on your own request");
