using Microsoft.EntityFrameworkCore;

namespace FormApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .Property(p => p.Email).HasMaxLength(255);
        modelBuilder.Entity<User>()
            .HasIndex(i => i.Email).IsUnique();
        modelBuilder.Entity<User>()
            .Property(p => p.GoogleSubject).HasMaxLength(25);
        modelBuilder.Entity<User>()
            .HasIndex(p => p.GoogleSubject).IsUnique();

        modelBuilder.Entity<Form>()
            .Property(p => p.Title).HasMaxLength(100);
        modelBuilder.Entity<Form>()
            .HasIndex(i => i.URL).IsUnique();
        modelBuilder.Entity<Form>()
            .HasOne(f => f.User)
            .WithMany(u => u.Forms)
            .HasForeignKey(f => f.UserId);

        modelBuilder.Entity<Question>()
            .Property(p => p.QuestionText).HasMaxLength(100);
        modelBuilder.Entity<Question>()
            .HasOne(q => q.QuestionType)
            .WithMany();
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne();
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Form)
            .WithMany(f => f.Questions); ///

        modelBuilder.Entity<Options>()
            .Property(p => p.OptionText).HasMaxLength(100);
        modelBuilder.Entity<Options>()
        .HasOne(o => o.Question)
        .WithMany(q => q.Options); //

        modelBuilder.Entity<QuestionType>()
            .Property(p => p.Type).HasMaxLength(25);
        modelBuilder.Entity<QuestionType>()
            .HasIndex(i => i.Type).IsUnique();

        modelBuilder.Entity<Response>()
            .HasOne(u => u.User)
            .WithMany();
        modelBuilder.Entity<Response>()
            .HasOne(f => f.Form)
            .WithMany();

        modelBuilder.Entity<Answer>()
            .HasOne(r => r.Response)
            .WithMany(a => a.Answers);
        modelBuilder.Entity<Answer>()
            .HasOne(r => r.Question)
            .WithMany();

        modelBuilder.Entity<SelectedOption>()
            .HasOne(a => a.Option)
            .WithMany(o => o.SelectedOptions);
        modelBuilder.Entity<SelectedOption>()
            .HasOne(a => a.Answer)
            .WithMany(a => a.SelectedOptions);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Options> Options { get; set; }
    public DbSet<QuestionType> QuestionTypes { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<SelectedOption> SelectedOptions { get; set; }
}