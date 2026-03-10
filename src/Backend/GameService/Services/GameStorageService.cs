using GameService.Models;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Services;

public class GameStorageService(DbContext context) : BasePostgresSqlStorageService<Game>(context);