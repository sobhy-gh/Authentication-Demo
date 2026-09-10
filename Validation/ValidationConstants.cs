namespace AuthenticationDemo.Validation
{
	public class ValidationConstants
	{
		public const string PasswordRegex =
	@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()\-_=+])[A-Za-z\d@$!%*?&#^()\-_=+]{8,64}$";
	}
}
