﻿using Microsoft.AspNetCore.Identity;

namespace DineEase.Models
{
	public class ApplicationUser : IdentityUser
	{
		public ICollection<Order>? Orders { get; set; }
	}
}
