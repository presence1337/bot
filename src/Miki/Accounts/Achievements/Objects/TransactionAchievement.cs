﻿using System;
using System.Threading.Tasks;

namespace Miki.Accounts.Achievements.Objects
{
	internal class TransactionAchievement : IAchievement
	{
		public Func<TransactionPacket, Task<bool>> CheckTransaction;

		public string Name { get; set; }
		public string ParentName { get; set; }
		public string Icon { get; set; }
		public int Points { get; set; }

		public async Task<bool> CheckAsync(BasePacket packet)
		{
			return await CheckTransaction(packet as TransactionPacket);
		}
	}
}