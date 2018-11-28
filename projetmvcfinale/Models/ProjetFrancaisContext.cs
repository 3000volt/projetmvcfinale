using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace projetmvcfinale.Models
{
    public partial class ProjetFrancaisContext : DbContext
    {
        private readonly string ConnectionString;
        public ProjetFrancaisContext(string connectionstring)
        {
            this.ConnectionString = connectionstring;
        }
        public ProjetFrancaisContext()
        {
        }

        public ProjetFrancaisContext(DbContextOptions<ProjetFrancaisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categorie> Categorie { get; set; }
        public virtual DbSet<ChoixDeReponse> ChoixDeReponse { get; set; }
        public virtual DbSet<Commentaires> Commentaires { get; set; }
        public virtual DbSet<Corrige> Corrige { get; set; }
        public virtual DbSet<Exercice> Exercice { get; set; }
        public virtual DbSet<LigneTestInteractif> LigneTestInteractif { get; set; }
        public virtual DbSet<Niveau> Niveau { get; set; }
        public virtual DbSet<NoteDeCours> NoteDeCours { get; set; }
        public virtual DbSet<SousCategorie> SousCategorie { get; set; }
        public virtual DbSet<Utilisateur> Utilisateur { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(" Server=localhost;Database=ProjetFrancais;User Id=sa;Password=sql");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categorie>(entity =>
            {
                entity.HasKey(e => e.IdCateg);

                entity.Property(e => e.IdCateg)
                    .HasColumnName("Id_Categ")
                    .ValueGeneratedNever();

                entity.Property(e => e.NomCategorie)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<ChoixDeReponse>(entity =>
            {
                entity.HasKey(e => e.IdChoix);

                entity.Property(e => e.IdChoix)
                    .HasColumnName("Id_Choix")
                    .ValueGeneratedNever();

                entity.Property(e => e.ChoixDeReponse1)
                    .IsRequired()
                    .HasColumnName("ChoixDeReponse")
                    .HasMaxLength(50);

                entity.Property(e => e.IdLigne).HasColumnName("id_ligne");

                entity.HasOne(d => d.IdLigneNavigation)
                    .WithMany(p => p.ChoixDeReponse)
                    .HasForeignKey(d => d.IdLigne)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChoixDeRe__id_li__52593CB8");
            });

            modelBuilder.Entity<Commentaires>(entity =>
            {
                entity.HasKey(e => e.IdCom);

                entity.Property(e => e.IdCom)
                    .HasColumnName("Id_com")
                    .ValueGeneratedNever();

                entity.Property(e => e.AdresseCourriel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateCommentaire).HasColumnType("date");

                entity.Property(e => e.TexteCom)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.AdresseCourrielNavigation)
                    .WithMany(p => p.Commentaires)
                    .HasForeignKey(d => d.AdresseCourriel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Commentai__Adres__48CFD27E");
            });

            modelBuilder.Entity<Corrige>(entity =>
            {
                entity.HasKey(e => e.Idcorrige);

                entity.Property(e => e.Idcorrige).ValueGeneratedNever();

                entity.Property(e => e.CorrigeDocNom)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateInsertion).HasColumnType("date");

                entity.Property(e => e.Lien).HasMaxLength(50);

                entity.HasOne(d => d.IdexerciceNavigation)
                    .WithMany(p => p.Corrige)
                    .HasForeignKey(d => d.Idexercice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Corrige__Idexerc__5165187F");
            });

            modelBuilder.Entity<Exercice>(entity =>
            {
                entity.HasKey(e => e.Idexercice);

                entity.Property(e => e.Idexercice).ValueGeneratedNever();

                entity.Property(e => e.AdresseCourriel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateInsertion).HasColumnType("date");

                entity.Property(e => e.IdCateg).HasColumnName("Id_Categ");

                entity.Property(e => e.IdDifficulte).HasColumnName("Id_difficulte");

                entity.Property(e => e.Lien).HasMaxLength(50);

                entity.Property(e => e.NomExercices)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TypeExercice)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.AdresseCourrielNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.AdresseCourriel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Exercice__Adress__4CA06362");

                entity.HasOne(d => d.IdCategNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.IdCateg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Exercice__Id_Cat__5070F446");

                entity.HasOne(d => d.IdDifficulteNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.IdDifficulte)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Exercice__Id_dif__4D94879B");

                entity.HasOne(d => d.IdDocumentNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.IdDocument)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Exercice__IdDocu__4F7CD00D");

                entity.HasOne(d => d.IdcorrigeNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.Idcorrige)
                    .HasConstraintName("FK__Exercice__Idcorr__4E88ABD4");
            });

            modelBuilder.Entity<LigneTestInteractif>(entity =>
            {
                entity.HasKey(e => e.IdLigne);

                entity.Property(e => e.IdLigne)
                    .HasColumnName("id_ligne")
                    .ValueGeneratedNever();

                entity.Property(e => e.Ligne)
                    .IsRequired()
                    .HasColumnName("ligne")
                    .HasColumnType("text");

                entity.HasOne(d => d.IdexerciceNavigation)
                    .WithMany(p => p.LigneTestInteractif)
                    .HasForeignKey(d => d.Idexercice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LigneTest__Idexe__534D60F1");
            });

            modelBuilder.Entity<Niveau>(entity =>
            {
                entity.HasKey(e => e.IdDifficulte);

                entity.Property(e => e.IdDifficulte)
                    .HasColumnName("Id_difficulte")
                    .ValueGeneratedNever();

                entity.Property(e => e.NiveauDifficulte)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<NoteDeCours>(entity =>
            {
                entity.HasKey(e => e.IdDocument);

                entity.Property(e => e.IdDocument).ValueGeneratedNever();

                entity.Property(e => e.AdresseCourriel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateInsertion).HasColumnType("date");

                entity.Property(e => e.IdCateg).HasColumnName("Id_Categ");

                entity.Property(e => e.Lien).HasMaxLength(50);

                entity.Property(e => e.NomNote)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.AdresseCourrielNavigation)
                    .WithMany(p => p.NoteDeCours)
                    .HasForeignKey(d => d.AdresseCourriel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDeCou__Adres__49C3F6B7");

                entity.HasOne(d => d.IdCategNavigation)
                    .WithMany(p => p.NoteDeCours)
                    .HasForeignKey(d => d.IdCateg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDeCou__Id_Ca__4AB81AF0");

                entity.HasOne(d => d.IdSousCategorieNavigation)
                    .WithMany(p => p.NoteDeCours)
                    .HasForeignKey(d => d.IdSousCategorie)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDeCou__IdSou__4BAC3F29");
            });

            modelBuilder.Entity<SousCategorie>(entity =>
            {
                entity.HasKey(e => e.IdSousCategorie);

                entity.Property(e => e.IdSousCategorie).ValueGeneratedNever();

                entity.Property(e => e.IdCateg).HasColumnName("Id_Categ");

                entity.Property(e => e.NomSousCategorie)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.IdCategNavigation)
                    .WithMany(p => p.SousCategorie)
                    .HasForeignKey(d => d.IdCateg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SousCateg__Id_Ca__5441852A");
            });

            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasKey(e => e.AdresseCourriel);

                entity.Property(e => e.AdresseCourriel)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Prenom)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.RegistrerDate).HasColumnType("date");
            });
        }
    }
}
