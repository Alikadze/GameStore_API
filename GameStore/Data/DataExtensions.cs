using System;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public static class DataExtensions
{
  public static async Task MigrateDbAsync(this WebApplication App)
  {
    using var scope = App.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
    await dbContext.Database.MigrateAsync();
  }
}
