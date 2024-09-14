using System;
using GameStore.Data;
using GameStore.DTOs;
using GameStore.Entities;
using GameStore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.EndPoints;

public static class GamesEndPoints
{
  const string GetGameEndpointName = "GetGame";

  private static readonly List<GameSummaryDTO> games = [
    new(1, "Cyberpunk 2077", "RPG", 200, new DateOnly(2020, 12, 10)),
    new(2, "The Witcher 3: Wild Hunt", "RPG", 100, new DateOnly(2015, 5, 19)),
    new(3, "Doom Eternal", "FPS", 150, new DateOnly(2020, 3, 20)),
    new(4, "Half-Life: Alyx", "FPS", 150, new DateOnly(2020, 3, 23)),
    new(5, "Death Stranding", "Action", 200.33M, new DateOnly(2019, 11, 8))
  ];

  public static RouteGroupBuilder MapGamesEndpoint(this WebApplication app) {
   
    var group = app.MapGroup("games").WithParameterValidation();

    //GET /games
    group.MapGet("/", async (GameStoreContext dbContext) => 
      await dbContext.Games.Include(game => game.Genre)
                     .Select(g => g.ToGameSummaryDto())
                     .AsNoTracking()
                     .ToListAsync()
    );

    //GET /games/{id}
    group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => {

      Game? game = await dbContext.Games.FindAsync(id);

      return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound();
    })
    .WithName(GetGameEndpointName);

    //POST /games
    group.MapPost("/", async (CreateGameDTO newGame, GameStoreContext dbContext) =>
      {
        Game game = newGame.ToEntity();

        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        return Results.CreatedAtRoute(
          GetGameEndpointName,
          new { id = game.Id },
          game.ToGameDetailsDto()
        );
      }
    );

    //PUT /games/{id}
    group.MapPut("/{id}", async (int id, UpdateGameDTO updatedGame, GameStoreContext dbContext) =>
      {
        var existingGame = await dbContext.Games.FindAsync(id);
          
        if (existingGame is null) {
          return Results.NotFound();
        }
        
        dbContext.Entry(existingGame)
                 .CurrentValues
                 .SetValues(updatedGame.ToEntity(id));

        await dbContext.SaveChangesAsync();

        return Results.NoContent();
      }
    );

    //DELETE /games/{id}
    group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
      {
        await dbContext.Games.Where(g => g.Id == id)
                             .ExecuteDeleteAsync();

        return Results.NoContent();
      }
    );

    return group;
  }
}
