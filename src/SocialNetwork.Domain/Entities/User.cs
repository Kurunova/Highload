﻿namespace SocialNetwork.Domain.Entities;

public class User
{
	public long Id { get; set; }
	public string Login { get; set; }
	public string PasswordHash { get; set; }
	public byte[] Salt { get; set; }
	public string FirstName { get; set; }
	public string SecondName { get; set; }
	public DateTime Birthdate { get; set; }
	public GenderType Gender { get; set; }
	public string City { get; set; }
    public string Hobbies { get; set; }
}