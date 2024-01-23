
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var assets = CreateAssets().ToDictionary(a => a.id);
var users = CreateUsers().ToDictionary(u => u.id);
var shares = CreateSharedAssets(assets, users);

app.UseHttpsRedirection();

app.MapPost("/login", ([Microsoft.AspNetCore.Mvc.FromBody] LoginParameters req) =>
{
    return TypedResults.Ok(new LoginResult(true, "", "-- TOKEN --"));
}).WithName("Login")
.WithDescription("Logs the user in with the given username and passwordhash - Returns the Bearer Token")
.WithOpenApi();

app.MapPost("/logout", () =>
{
    return Results.Ok();
}).WithName("logout")
.WithDescription("Logs the current user out and delete his Bearer Token")
.WithOpenApi();

app.MapGet("/loginSalt", ([Microsoft.AspNetCore.Mvc.FromBody] LoginSaltRequest req) =>
{
    return TypedResults.Ok("this is the salt for " + req.username);
}).WithName("LoginSalt")
.WithDescription("Returns the salt that should be use to hash the password (sha256)")
.WithOpenApi();

app.MapGet("/register", ([Microsoft.AspNetCore.Mvc.FromBody] RegisterUserRequest req) =>
{
    return TypedResults.Ok();
}).WithName("Register a User")
.WithDescription("Register a User with the given username, passwordhash and mail address")
.WithOpenApi();

app.MapGet("/users", () =>
{
    return TypedResults.Ok(users.Values);
}).WithName("Get all Users")
.WithDescription("Returns all registered users")
.WithOpenApi();

// Return  all assets in the assets dictionary
app.MapGet("/assets", () =>
{
    return TypedResults.Ok(assets.Values);
}).WithName("List Assets")
.WithDescription("Returns all existing assets")
.WithOpenApi();

// Return the details of a single asset
app.MapGet("/assets/{id}", (string id) =>
{
    return TypedResults.Ok(assets[id]);
}).WithName("Get one Asset")
.WithDescription("Retunrs the chosen Asset with the given id")
.WithOpenApi();

app.MapGet("/sharedassets/{id}", (string id) =>
{
    return TypedResults.Ok(shares[id]);
}).WithName("Get Shared Assets")
.WithDescription("Returns the user id´s of the given asset is shared with")
.WithOpenApi();

app.MapPut("/updatesharedassets/{id}", (string id, List<string> users) =>
{
    shares[id] = users; 
    return TypedResults.Ok();
}).WithName("Update Shared Assets")
.WithDescription("Update the user id´s (shared with) of the given asset")
.WithOpenApi();

// Create a new Asset
app.MapPost("/assets", (Asset asset) =>
{
    if (assets.ContainsKey(asset.id))
    {
        return Results.BadRequest("Asset already exists");
    }
    assets.Add(asset.id, asset);
    return Results.Ok();
}).WithName("Create Asset")
.WithDescription("Create a new asset with the given parameter")
.WithOpenApi();

// Update an existing Asset
app.MapPut("/assets", (Asset asset) =>
{
    if (!assets.ContainsKey(asset.id))
    {
        return Results.NotFound();
    }
    assets[asset.id] = asset;
    return Results.Ok(asset);
}).WithName("Update Asset")
.WithDescription("Update an existing asset with the new or changed parameters")
.WithOpenApi();

// Delete an existing Asset
app.MapDelete("/assets/{id}", (string id) =>
{
    if (!assets.ContainsKey(id))
    {
        return Results.NotFound();
    }
    assets.Remove(id);
    return Results.Ok();
}).WithName("Delete Asset")
.WithDescription("Delete the asset with the given id")
.WithOpenApi();

app.Run();

static List<Asset> CreateAssets()
{
    var assets = new List<Asset>();
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Tree", "A small tree", "/assets/tree.model", "Clemens", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Rock", "A large rock", "/assets/rock.model", "Eleanor", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Sword", "A sharp sword", "/assets/sword.model", "Gideon", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Shield", "A sturdy shield", "/assets/shield.model", "Isabella", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Potion", "A healing potion", "/assets/potion.model", "Felix", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Castle", "A majestic castle", "/assets/castle.model", "Sophia", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Dragon", "A fearsome dragon", "/assets/dragon.model", "Maximus", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Gem", "A precious gem", "/assets/gem.model", "Aria", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Scroll", "A magical scroll", "/assets/scroll.model", "Darius", GetCurrentDate()));
    assets.Add(new Asset(Guid.NewGuid().ToString(), "Helmet", "A protective helmet", "/assets/helmet.model", "Luna", GetCurrentDate()));
    return assets;
}

static List<User> CreateUsers()
{
    List<User> users = new List<User>();
    users.Add(new User(Guid.NewGuid().ToString(), "Fritzi"));
    users.Add(new User(Guid.NewGuid().ToString(), "Franzi"));
    users.Add(new User(Guid.NewGuid().ToString(), "Lenny"));
    users.Add(new User(Guid.NewGuid().ToString(), "Jenny"));
    users.Add(new User(Guid.NewGuid().ToString(), "Tim"));
    users.Add(new User(Guid.NewGuid().ToString(), "Kim"));
    users.Add(new User(Guid.NewGuid().ToString(), "Danny"));
    users.Add(new User(Guid.NewGuid().ToString(), "Fanny"));
    users.Add(new User(Guid.NewGuid().ToString(), "Hansi"));
    users.Add(new User(Guid.NewGuid().ToString(), "Schimpansi"));
    return users;
}

Dictionary<string, List<string>> CreateSharedAssets(Dictionary<string, Asset> assets, Dictionary<string, User> users)
{
    Random rnd = new Random();
    var shares = new Dictionary<string, List<string>>();


    foreach (var asset in assets.Values)
    {
        var userIds = new List<string>();
        for (int i = 0; i < 3; i++)
        {   
            var userIdx = rnd.Next(users.Count);
            var uid = users.Values.ElementAt(userIdx);

            if(!userIds.Contains(uid.id))
            {
                userIds.Add(uid.id);
            }
        }
        shares.Add(asset.id, userIds);
    }


    return shares;
}

static string GetCurrentDate()
{
    return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}



//Klassen die nur als Datenhalter fungiert - keine Methoden implementiert hat
record LoginParameters(string username, string passwordHash);
record LoginResult(bool success, string message, string token);
record LoginSaltResponse(string salt);
record LoginSaltRequest(string username);
record RegisterUserRequest(string username, string passwordHash, string email);
record Asset(string id, string name, string description, string path, string owner, string created);
record User(string id, string name);

