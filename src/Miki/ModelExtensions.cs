﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miki.Bot.Models;
using Miki.Bot.Models.Models.User;
using Miki.Discord.Common;
using Miki.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Miki.Services.Marriages;

namespace Miki
{
    public static class ModelExtensions
	{
        public static async Task BanAsync(this User user, DbContext context)
        {
            // TODO: replace for IContext
            MarriageService repository = new MarriageService(new UnitOfWork(context));

            User u = await context.Set<User>().FindAsync(user.Id);

            await repository.DivorceAllMarriagesAsync(user.Id);
            await repository.DeclineAllProposalsAsync(user.Id);

            context.Set<CommandUsage>().RemoveRange(
                await context.Set<CommandUsage>().Where(x => x.UserId == user.Id).ToListAsync()
            );

            context.Set<Achievement>().RemoveRange(
                await context.Set<Achievement>().Where(x => x.UserId == user.Id).ToListAsync()
            );

            context.Set<LocalExperience>().RemoveRange(
                await context.Set<LocalExperience>().Where(x => x.UserId == user.Id).ToListAsync()
            );

            await context.Set<IsBanned>()
                .AddAsync(new IsBanned
            {
                UserId = user.Id,
                TimeOfBan = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(10)
            });

            u.Total_Commands = 0;
            u.Total_Experience = 0;
            u.MarriageSlots = 0;
            u.Currency = 0;
            u.Reputation = 0;

            await context.SaveChangesAsync();
        }

		public static async Task<IDiscordRole> GetRoleAsync(this LevelRole role, IDiscordGuild guild)
		{
			return await guild.GetRoleAsync((ulong)role.RoleId);
		}
	}
}