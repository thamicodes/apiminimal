
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TarefaDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.MapGet("/", (TarefaDb db) =>
{
      var lista = new List<Tarefa>{
new Tarefa { Nome = "Analisar Logs", FoiConcluida = true },
new Tarefa { Nome = "Cadastrar Cartão de Suporte no Kanban", FoiConcluida = false },
new Tarefa { Nome = "Fazer Code Review", FoiConcluida = false },
new Tarefa { Nome = "Modelar Banco de Dados", FoiConcluida = true }};
        db.Tarefas.AddRange(lista);

        db.SaveChanges();
    return Results.Ok();
});

app.MapGet("/data", () => new DateTime());

app.MapGet("/tarefas", async (TarefaDb db) =>
    await db.Tarefas.ToListAsync());

app.MapGet("/tarefas/concluidas", async (TarefaDb db) =>
    await db.Tarefas.Where(t => t.FoiConcluida).ToListAsync());

app.MapGet("/tarefas/nao-concluidas", async (TarefaDb db) =>
    await db.Tarefas.Where(t => !t.FoiConcluida).ToListAsync());

app.MapGet("/tarefa/{id}", async (int id, TarefaDb db) =>
    await db.Tarefas.FindAsync(id)
        is Tarefa todo
            ? Results.Ok(todo)
           // : Results.NotFound());
           : Results.NoContent());//204

app.MapGet("/tarefa/por-nome/{nome}", async (string nome, TarefaDb db) =>
await db.Tarefas.Where(t => t.Nome == nome).ToListAsync()
is List<Tarefa> todos
? Results.Ok(todos)
// : Results.NotFound());
: Results.NoContent());//204

app.MapPost("/tarefas", async (Tarefa tarefa, TarefaDb db) =>
{
    if (!tarefa.IsValid())
        return Results.UnprocessableEntity();//422

    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();

    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});


app.MapPut("/tarefas/{id}", async (int id, Tarefa tarefaAlterada, TarefaDb db) =>
{
    if (!tarefaAlterada.IsValid())
        return Results.UnprocessableEntity();

    var tarefa = await db.Tarefas.FindAsync(id);

    //if (todo is null) return Results.NotFound();
    if (tarefa is null) return Results.NoContent();//204

    tarefa.Nome = tarefaAlterada.Nome;
    tarefa.FoiConcluida = tarefaAlterada.FoiConcluida;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/tarefas/{id}", async (int id, TarefaDb db) =>
{
    if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
    {
        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
        return Results.Ok(tarefa);
    }
    //return Results.BadRequest();//400
    return Results.NotFound();//404
                              // return Results.NoContent();//204
});


app.Run();

class Tarefa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public bool FoiConcluida { get; set; }
    public bool IsValid()
    {
        if (!string.IsNullOrEmpty(Nome))
            return true;

        return false;
    }
}


class TarefaDb : DbContext
{
    public TarefaDb(DbContextOptions<TarefaDb> options)
        : base(options)
    {

        // this.Add(new Todo{Nome = "Analisar Logs", IsComplete = true});
        // this.SaveChanges();

    }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {

    //     modelBuilder.Entity<Tarefa>().HasData(new Tarefa { Nome = "Analisar Logs", FoiConcluida = true });
    //     modelBuilder.Entity<Tarefa>().HasData(new Tarefa { Nome = "Cadastrar Cartão de Suporte no Kanban", FoiConcluida = false });
    //     modelBuilder.Entity<Tarefa>().HasData(new Tarefa { Nome = "Fazer Code Review", FoiConcluida = false });
    //     modelBuilder.Entity<Tarefa>().HasData(new Tarefa { Nome = "Modelar Banco de Dados", FoiConcluida = true });
    // }


}
static class teste
{
    public static void AddTestData(TarefaDb context)
    {


        var lista = new List<Tarefa>{
new Tarefa { Nome = "Analisar Logs", FoiConcluida = true },
new Tarefa { Nome = "Cadastrar Cartão de Suporte no Kanban", FoiConcluida = false },
new Tarefa { Nome = "Fazer Code Review", FoiConcluida = false },
new Tarefa { Nome = "Modelar Banco de Dados", FoiConcluida = true }};
        context.Tarefas.AddRange(lista);

        context.SaveChanges();
    }
}