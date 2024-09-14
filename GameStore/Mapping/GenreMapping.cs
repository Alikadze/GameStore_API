using System;
using GameStore.DTOs;
using GameStore.Entities;

namespace GameStore.Mapping;

public static class GenreMapping
{
  public static GenreDTO ToDTO(this Genre genre)
  {
    return new GenreDTO(genre.Id, genre.Name);
  }
}
