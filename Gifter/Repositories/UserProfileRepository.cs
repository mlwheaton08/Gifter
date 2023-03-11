using Gifter.Models;
using Gifter.Utils;
using Microsoft.Extensions.Hosting;
using System;

namespace Gifter.Repositories;

public class UserProfileRepository : BaseRepository, IUserProfileRepository
{
    public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

    public List<UserProfile> GetAll()
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, up.Name, Email, ImageUrl, Bio, DateCreated
                                  FROM UserProfile up";
                var reader = cmd.ExecuteReader();

                var profiles = new List<UserProfile>();
                while (reader.Read())
                {
                    profiles.Add(new UserProfile()
                    {
                        Id = DbUtils.GetInt(reader, "Id"),
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                    });
                }

                reader.Close();
                return profiles;
            }
        }
    }

    public List<UserProfile> GetAllWithPosts()
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT
	                                    up.Id as UserId,
	                                    up.Name as UserName,
	                                    up.Email,
	                                    up.ImageUrl as UserImageUrl,
	                                    up.Bio,
	                                    up.DateCreated as UserProfileDateCreated,
	                                    p.Id as PostId,
	                                    p.Title as PostTitle,
	                                    p.ImageUrl as PostImageUrl,
	                                    p.Caption as PostCaption,
	                                    p.DateCreated as PostDateCreated
                                    FROM UserProfile up
                                    LEFT JOIN Post p
                                    ON up.Id = p.UserProfileId";

                var reader = cmd.ExecuteReader();

                var userProfiles = new List<UserProfile>();
                while (reader.Read())
                {
                    var userProfileId = DbUtils.GetInt(reader, "UserId");

                    var existingUserProfile = userProfiles.FirstOrDefault(u => u.Id == userProfileId);
                    if (existingUserProfile == null)
                    {
                        existingUserProfile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "UserId"),
                            Name = DbUtils.GetString(reader, "UserName"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "UserImageUrl"),
                            DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                            Posts = new List<Post>()
                        };

                    userProfiles.Add(existingUserProfile);
                    }

                    if (DbUtils.IsNotDbNull(reader, "PostId"))
                    {
                        existingUserProfile.Posts.Add(new Post()
                        {
                            Id = DbUtils.GetInt(reader, "PostId"),
                            Title = DbUtils.GetString(reader, "PostTitle"),
                            ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                            Caption = DbUtils.GetString(reader, "PostCaption"),
                            DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                            UserProfileId = DbUtils.GetInt(reader, "UserId"),
                        });
                    }
                }

                reader.Close();
                return userProfiles;
            }
        }
    }

    public UserProfile GetById(int id)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, up.Name, Email, ImageUrl, Bio, DateCreated
                                    FROM UserProfile up
                                    WHERE Id = @Id";

                DbUtils.AddParameter(cmd, "@Id", id);

                var reader = cmd.ExecuteReader();

                UserProfile profile = null;
                if (reader.Read())
                {
                    profile = new UserProfile()
                    {
                        Id = DbUtils.GetInt(reader, "Id"),
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                    };
                }

                reader.Close();
                return profile;
            }
        }
    }

    public UserProfile GetByIdWithPosts(int id)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT
	                                    up.Id as UserId,
	                                    up.Name as UserName,
	                                    up.Email,
	                                    up.ImageUrl as UserImageUrl,
	                                    up.Bio,
	                                    up.DateCreated as UserProfileDateCreated,
	                                    p.Id as PostId,
	                                    p.Title as PostTitle,
	                                    p.ImageUrl as PostImageUrl,
	                                    p.Caption as PostCaption,
	                                    p.DateCreated as PostDateCreated
                                    FROM UserProfile up
                                    LEFT JOIN Post p
                                    ON up.Id = p.UserProfileId
                                    WHERE up.Id = @Id";

                DbUtils.AddParameter(cmd, "@Id", id);

                var reader = cmd.ExecuteReader();

                UserProfile profile = null;

                while (reader.Read())
                {
                    if (profile == null)
                    {
                        profile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "UserId"),
                            Name = DbUtils.GetString(reader, "UserName"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "UserImageUrl"),
                            DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                            Posts = new List<Post>()
                        };
                    }

                    if (DbUtils.IsNotDbNull(reader, "PostId"))
                    {
                        profile.Posts.Add(new Post()
                        {
                            Id = DbUtils.GetInt(reader, "PostId"),
                            Title = DbUtils.GetString(reader, "PostTitle"),
                            ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                            Caption = DbUtils.GetString(reader, "PostCaption"),
                            DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                            UserProfileId = DbUtils.GetInt(reader, "UserId"),
                        });
                    }
                }

                reader.Close();
                return profile;
            }
        }
    }

    public void Add(UserProfile profile)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO UserProfile (
                                        Name,
                                        Email,
                                        ImageUrl,
                                        Bio,
                                        DateCreated
                                    )
                                    OUTPUT INSERTED.ID
                                    VALUES (
                                        @Name,
                                        @Email,
                                        @ImageUrl,
                                        @Bio,
                                        CONVERT(datetime, @DateCreated)
                                    )";

                DbUtils.AddParameter(cmd, "@Name", profile.Name);
                DbUtils.AddParameter(cmd, "@Email", profile.Email);
                DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);
                DbUtils.AddParameter(cmd, "@Bio", profile.Bio);
                DbUtils.AddParameter(cmd, "@DateCreated", profile.DateCreated);

                profile.Id = (int)cmd.ExecuteScalar();
            }
        }
    }

    public void Update(UserProfile profile)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE UserProfile
                                        SET Name = @Name,
                                        Email = @Email,
                                        ImageUrl = @ImageUrl,
                                        Bio = @Bio,
                                        DateCreated = CONVERT (datetime, @DateCreated)
                                    WHERE Id = @Id";

                DbUtils.AddParameter(cmd, "@Name", profile.Name);
                DbUtils.AddParameter(cmd, "@Email", profile.Email);
                DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);
                DbUtils.AddParameter(cmd, "@Bio", profile.Bio);
                DbUtils.AddParameter(cmd, "@DateCreated", profile.DateCreated);
                DbUtils.AddParameter(cmd, "@Id", profile.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void Delete(int id)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                DbUtils.AddParameter(cmd, "@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
