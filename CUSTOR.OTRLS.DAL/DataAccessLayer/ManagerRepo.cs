﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSTOR.OTRLS.Core
{
    public class ManagerRepository
    {
        OTRLSDbContext context;
        public ManagerRepository(OTRLSDbContext _context)
        {
            context = _context;
        }
        public async Task<ManagerDTO> GetManager(int managerId)
        {
            //try
            //{
                Manager manager = null;
                Address add = null;

                int id = managerId;
                manager = await context.Manager
                    .FirstOrDefaultAsync(cus => cus.ManagerId == id);

                add = await context.Address
                    .FirstOrDefaultAsync(a => a.ParentId == id && a.AddressType == (int)AddressType.eManager);

                return ManagerHelper.GetManagerDTO(manager, add);
            //}
            //catch (Exception ex)
            //{
            //    string s = ex.Message;
            //    throw new Exception(ex.InnerException.ToString());
            //}

        }
        public async Task<ManagerDTO> SaveManager(ManagerDTO postedManager)
        {
            bool isUpdate = (postedManager.ManagerId > 0); // New or Update?
            // map DTO to manager entity 
            Manager mgr = ManagerHelper.GetManager(postedManager);
            mgr.CreatedDate = DateTime.Now;

            // We should always use transaction for operations involving two or more entities
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // first save manager entity
                    if (isUpdate)
                    {
                        mgr.ManagerId = postedManager.ManagerId;
                        context.Manager.Update(mgr);
                        context.SaveChanges();

                    }
                    else
                    {
                        context.Manager.Add(mgr);
                        context.SaveChanges();
                    }
                    // then save address entity
                    Address address = ManagerHelper.GetAddress(postedManager);
                    address.ParentId = mgr.ManagerId;

                    if (isUpdate)
                    {
                        address.AddressId = postedManager.AddressId;
                        context.Address.Update(address);
                        context.SaveChanges();
                    }
                    else
                    {
                        address.AddressId = 0;
                        context.Address.Add(address);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

                transaction.Commit();
                postedManager.ManagerId = mgr.ManagerId; //used for image filename
                return postedManager;
            }
        }
        public async Task<PagedResult<ManagerListDTO>> GetManagerByCustomerId2(int CustomerId, int PageCount = 10, int PageNumber = 1)
        {
            int id = CustomerId;
            IEnumerable<ManagerListDTO> query = await context.Manager
                .Where(m => m.CustomerId == CustomerId)
                .Select(mgr => new ManagerListDTO
                {
                    ManagerId = mgr.ManagerId,
                    FullName = mgr.FirstName + " " + mgr.FatherName + " " + mgr.GrandName,
                    FullNameEng = mgr.FirstNameEng + " " + mgr.FatherNameEng + " " + mgr.GrandNameEng
                })
                .Paging(PageCount, PageNumber)
                .ToListAsync();

            return new PagedResult<ManagerListDTO>()
            {
                Items = query,
                ItemsCount = context.Manager.Count()
            };

        }

        public async Task<PagedResult<ManagerListDTO>> GetManagerByCustomerId(int CustomerId, int PageNumber = 1, int PageCount = 10 )
        {
            int id = CustomerId;
            IEnumerable<ManagerListDTO> query = await context.Manager
                .Where(m => m.CustomerId == CustomerId)
                .Select(mgr => new ManagerListDTO
                {
                    ManagerId = mgr.ManagerId,
                    FullName = mgr.FirstName + " " + mgr.FatherName + " " + mgr.GrandName,
                    FullNameEng = mgr.FirstNameEng + " " + mgr.FatherNameEng + " " + mgr.GrandNameEng,
                    Gender = mgr.Gender,
                    Nationality = mgr.Nationality.ToString()
                })
                .Paging(PageCount, PageNumber)
                .ToListAsync();
             
            return new PagedResult<ManagerListDTO>()
            {
                Items = query, 
                ItemsCount = context.Manager.Count()
            };

        }
        public async Task<PagedResult<ManagerListDTO>> GetManagerByCustomerId2(QueryParameters queryParameter)
        {
             
            string natFieldName = StaticDataHelper.GetNationalityFieldName(queryParameter.Lang);
            string query1 = $@"(SELECT ManagerId, CustomerId, FirstName + ' ' + FatherName + ' ' + GrandName as FullName,
		                             FirstNameEng + ' ' + FatherNameEng + ' ' + GrandNameEng as FullNameEng, Gender,
		                            'Nationality' = (SELECT {natFieldName} FROM dbo.[Lookup] WHERE LookupId = M.Nationality)
                                    FROM [dbo].[Manager] M)";
    
            IEnumerable<ManagerListDTO> query = await context.ManagerListDTO
                .Where(m => m.CustomerId == queryParameter.CustomerId)
                .FromSql(query1)
                .Paging(queryParameter.PageCount, queryParameter.PageNumber)
                .ToListAsync();

            return new PagedResult<ManagerListDTO>()
            {
                Items = query,
                ItemsCount = context.Manager.Count()
            };

        }
        public async Task<List<ManagerListDTO>> GetManagerByCustomerId(int CustomerId)
        {
            int id = CustomerId;
            return await context.Manager
                .Where(m => m.CustomerId == CustomerId)
                .Select(mgr => new ManagerListDTO
                {
                    ManagerId = mgr.ManagerId,
                    FullName = mgr.FirstName + " " + mgr.FatherName + " " + mgr.GrandName,
                    FullNameEng = mgr.FirstNameEng + " " + mgr.FatherNameEng + " " + mgr.GrandNameEng
                })
                .ToListAsync();
        }
        public async Task<bool> DeleteManager(int managerId)
        {
            Manager manager = await context.Manager.AsNoTracking().Where(m => m.ManagerId == managerId).FirstOrDefaultAsync();

            if (manager != null)
            {
                context.Manager.Remove(manager);
                await context.SaveChangesAsync();
            }
            return true;
        }
    }
}
