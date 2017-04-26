using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EntryProject.UI.ViewModels;
using Microsoft.Extensions.Options;
using System.IO;

namespace EntryProject.UI.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private const int InsertInOneCallLimit = 1000;
        private const string TempFolderName = "Temp";
        private const string BulkInsertFileExtension = "csv";
        private const string BulkInsertStatementFormat = @"
            BULK INSERT
		        GroupUsers
	        FROM '{0}'
	        WITH
	        (
		        CHECK_CONSTRAINTS,
		        ROWS_PER_BATCH = 500,
		        FIELDTERMINATOR =','
	        );
        ";

        private readonly DataModels.TestDBContext _context;

        public GroupRepository(DataModels.TestDBContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Groups.AsNoTracking().AnyAsync(g => g.Name == name);
        }

        public async Task<IEnumerable<GroupType>> ListGroupTypesAsync()
        {
            IEnumerable<GroupType> types = (await _context.GroupTypes.AsNoTracking().ToListAsync())
                .Select(DbGroupTypeToGroupType);

            return types;
        }

        public async Task CreateAsync(string name, string description, int groupTypeId, string[] userPhoneNumbers)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                string filepath = null;

                try
                {
                    var group = await _context.Groups.AddAsync(new DataModels.Group
                    {
                        Name = name,
                        Description = description,
                        CreatedDate = DateTime.UtcNow,
                        GroupTypeId = groupTypeId
                    });

                    if (userPhoneNumbers.Length <= InsertInOneCallLimit)
                    {
                        var groupUsers = userPhoneNumbers.Select(number => new DataModels.GroupUser
                        {
                            PhoneNumber = number,
                            GroupId = group.Entity.Id
                        }).ToArray();

                        await _context.GroupUsers.AddRangeAsync(groupUsers);
                    }

                    await _context.SaveChangesAsync();

                    if (userPhoneNumbers.Length > InsertInOneCallLimit)
                    {
                        // Good enough (may be?) for this case, without a knowledge of the whole product
                        filepath = GetTempFilePath();

                        File.WriteAllLines(
                            filepath,
                            userPhoneNumbers.Select(number => $"0,{number},{group.Entity.Id}")
                        );

                        string commandText = string.Format(BulkInsertStatementFormat, filepath);

                        _context.Database.ExecuteSqlCommand(commandText);

                        // Let it throw an exception and return 500, if it failes
                        transaction.Commit();
                    }
                }
                finally
                {
                    if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                }
            }
        }

        private GroupType DbGroupTypeToGroupType(DataModels.GroupType dbGroupType)
        {
            return new GroupType
            {
                Id = dbGroupType.Id,
                Name = dbGroupType.Name
            };
        }

        private string GetTempFilePath()
        {
            int guidSize = 16;
            byte[] bytes = new byte[guidSize];

            new Random().NextBytes(bytes);

            var guid = new Guid(bytes);

            string relativePath = Path.Combine(AppContext.BaseDirectory, TempFolderName);

            if (!Directory.Exists(relativePath))
            {
                Directory.CreateDirectory(relativePath);
            }

            string filepath = Path.Combine(relativePath, $"{guid.ToString()}.{BulkInsertFileExtension}");

            return filepath;
        }

    }
}