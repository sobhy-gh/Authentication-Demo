namespace AuthenticationDemo.Dtos
{
	public record AuthResponse(

		string AccessToken,
		string RefreshToken
	); 
}
