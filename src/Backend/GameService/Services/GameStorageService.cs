using GameService.Models;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Services;

public interface IGameStorageService : IPostgresSqlStorageService<Game>;
public class GameStorageService(DbContext context) : BasePostgresSqlStorageService<Game>(context), IGameStorageService;