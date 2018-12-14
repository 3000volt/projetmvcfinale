using System;
using System.Data;
using System.Data.SqlClient;
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
        public virtual DbSet<Commentaires> Commentaires { get; set; }
        public virtual DbSet<Corrige> Corrige { get; set; }
        public virtual DbSet<Exercice> Exercice { get; set; }
        public virtual DbSet<Niveau> Niveau { get; set; }
        public virtual DbSet<NoteDeCours> NoteDeCours { get; set; }
        public virtual DbSet<SousCategorie> SousCategorie { get; set; }
        public virtual DbSet<Utilisateur> Utilisateur { get; set; }

        public void updatelien(string lien,int id)
        {
            //utiliser le connectionString pour pouvoir affecter la BD
            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                //Requete pour ajouter un livre
                

                string sqlStr = "UPDATE Exercice SET Lien = @lien WHERE Idexercice=@idexo";
                //Code pour affecter la BD
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.Parameters.AddWithValue("lien",lien);
                cmd.Parameters.AddWithValue("idexo", id);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(" Server=localhost;Database=ProjetFrancais ;User Id=sa;Password=sql");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categorie>(entity =>
            {
                entity.HasKey(e => e.IdCateg);

                entity.Property(e => e.IdCateg).HasColumnName("Id_Categ");

                entity.Property(e => e.NomCategorie)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Commentaires>(entity =>
            {
                entity.HasKey(e => e.IdCom);

                entity.Property(e => e.IdCom).HasColumnName("Id_com");

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
                    .HasConstraintName("FK__Commentai__Adres__44FF419A");
            });

            modelBuilder.Entity<Corrige>(entity =>
            {
                entity.HasKey(e => e.Idcorrige);

                entity.Property(e => e.CorrigeDocNom)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateInsertion).HasColumnType("date");

                entity.Property(e => e.Lien).HasMaxLength(50);

                entity.HasOne(d => d.IdexerciceNavigation)
                    .WithMany(p => p.Corrige)
                    .HasForeignKey(d => d.Idexercice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Corrige__Idexerc__4D94879B");
            });

            modelBuilder.Entity<Exercice>(entity =>
            {
                entity.HasKey(e => e.Idexercice);

                entity.Property(e => e.AdresseCourriel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateInsertion).HasColumnType("date");

                entity.Property(e => e.ExercicesInt).HasColumnType("text");

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
                    .HasConstraintName("FK__Exercice__Adress__48CFD27E");

                entity.HasOne(d => d.IdCategNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.IdCateg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Exercice__Id_Cat__4CA06362");

                entity.HasOne(d => d.IdDifficulteNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.IdDifficulte)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Exercice__Id_dif__49C3F6B7");

                entity.HasOne(d => d.IdDocumentNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.IdDocument)
                    .HasConstraintName("FK__Exercice__IdDocu__4BAC3F29");

                entity.HasOne(d => d.IdcorrigeNavigation)
                    .WithMany(p => p.Exercice)
                    .HasForeignKey(d => d.Idcorrige)
                    .HasConstraintName("FK__Exercice__Idcorr__4AB81AF0");
            });

            modelBuilder.Entity<Niveau>(entity =>
            {
                entity.HasKey(e => e.IdDifficulte);

                entity.Property(e => e.IdDifficulte).HasColumnName("Id_difficulte");

                entity.Property(e => e.NiveauDifficulte)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<NoteDeCours>(entity =>
            {
                entity.HasKey(e => e.IdDocument);

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
                    .HasConstraintName("FK__NoteDeCou__Adres__45F365D3");

                entity.HasOne(d => d.IdCategNavigation)
                    .WithMany(p => p.NoteDeCours)
                    .HasForeignKey(d => d.IdCateg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDeCou__Id_Ca__46E78A0C");

                entity.HasOne(d => d.IdSousCategorieNavigation)
                    .WithMany(p => p.NoteDeCours)
                    .HasForeignKey(d => d.IdSousCategorie)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDeCou__IdSou__47DBAE45");
            });

            modelBuilder.Entity<SousCategorie>(entity =>
            {
                entity.HasKey(e => e.IdSousCategorie);

                entity.Property(e => e.IdCateg).HasColumnName("Id_Categ");

                entity.Property(e => e.NomSousCategorie)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.IdCategNavigation)
                    .WithMany(p => p.SousCategorie)
                    .HasForeignKey(d => d.IdCateg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SousCateg__Id_Ca__4E88ABD4");
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
