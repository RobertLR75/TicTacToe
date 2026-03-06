// using GameService.Models;
// using Npgsql;
// using SharedLibrary.PostgreSQL;
//
// namespace GameService.Persistence;
//
// public sealed class CreatedGamesSpecification : IPostgreSqlSpecification<GameModel>
// {
//     public CreatedGamesSpecification(GameStatus status, int page, int pageSize)
//     {
//         Status = status;
//         Page = page < 1 ? 1 : page;
//         PageSize = pageSize < 1 ? 50 : pageSize;
//     }
//
//     public GameStatus Status { get; }
//     public int Page { get; }
//     public int PageSize { get; }
//
//     public string Sql =>
//         """
//         SELECT gm.id,
//                gm.status,
//                gm.created_at_utc,
//                gm.updated_at_utc,
//                p1.id AS player1_id,
//                p1.name AS player1_name,
//                p2.id AS player2_id,
//                p2.name AS player2_name
//         FROM game_model gm
//         INNER JOIN player_model p1 ON p1.id = gm.player1_id
//         LEFT JOIN player_model p2 ON p2.id = gm.player2_id
//         WHERE gm.status = @status
//         ORDER BY gm.created_at_utc DESC
//         OFFSET @offset LIMIT @limit;
//         """;
//
//     public IReadOnlyDictionary<string, object?> Parameters => new Dictionary<string, object?>
//     {
//         ["status"] = Status.ToString(),
//         ["offset"] = (Page - 1) * PageSize,
//         ["limit"] = PageSize
//     };
//
//     public GameModel Map(NpgsqlDataReader reader)
//     {
//         var player2Id = reader.IsDBNull(reader.GetOrdinal("player2_id")) ? null : reader.GetString(reader.GetOrdinal("player2_id"));
//         var player2Name = reader.IsDBNull(reader.GetOrdinal("player2_name")) ? null : reader.GetString(reader.GetOrdinal("player2_name"));
//         DateTime? updatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at_utc"))
//             ? null
//             : reader.GetFieldValue<DateTime>(reader.GetOrdinal("updated_at_utc"));
//
//         return new GameModel
//         {
//             Id = reader.GetString(reader.GetOrdinal("id")),
//             Status = Enum.TryParse<GameStatus>(reader.GetString(reader.GetOrdinal("status")), out var status)
//                 ? status
//                 : GameStatus.Created,
//             CreatedAt = reader.GetFieldValue<DateTime>(reader.GetOrdinal("created_at_utc")),
//             UpdatedAt = updatedAt,
//             Player1 = new PlayerModel
//             {
//                 Id = reader.GetString(reader.GetOrdinal("player1_id")),
//                 Name = reader.GetString(reader.GetOrdinal("player1_name"))
//             },
//             Player2 = player2Id is null || player2Name is null
//                 ? null
//                 : new PlayerModel
//                 {
//                     Id = player2Id,
//                     Name = player2Name
//                 }
//         };
//     }
// }
