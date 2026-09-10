namespace AuthenticationDemo.Entities
{
	public class User
	{
	    public int Id { get; set; } 
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty; 
		public string HashedPassword { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } 
		public ICollection<UserRole> UserRoles { get; set; } = [];
		public ICollection<RefreshToken> RefreshTokens { get; set; } = []; 

	}
}
