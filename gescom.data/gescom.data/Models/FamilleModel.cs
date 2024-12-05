using System;
using System.Collections.Generic;
using System.Linq;

namespace gescom.data.Models
{
    public static class FamilleHelpers
    {
        public static bool Create(string code, string nom)
        {
            var model = new FamilleModel { Code = code, Nom = nom };
            var repository = new FamilleRepository();
            return repository.Create(model);
        }

        public static FamilleItem Get(long id)
        {
            var result = new FamilleItem();
            foreach (FamilleItem item in GetList().Where(item => item.Id == id))
            {
                result = item;
            }
            return result;
        }

        public static string GetCode(long id)
        {
            string result = null;
            foreach (FamilleItem item in GetList().Where(item => item.Id == id))
            {
                result = item.Code;
            }
            return result;
        }

        public static long GetId(string name)
        {
            long result = -1;
            foreach (FamilleItem item in GetList().Where(item => item.Nom == name))
            {
                result = item.Id;
            }
            return result;
        }

        public static List<FamilleItem> GetList()
        {
            var cart = new DataGescomDataContext();
            return cart.FamilleItems.ToList();
        }

        public static List<string> GetListFamilles()
        {
            return GetList().Select(item => item.Nom).ToList();
        }

        public static string GetName(long id)
        {
            string result = null;
            foreach (FamilleItem item in GetList().Where(item => item.Id == id))
            {
                result = item.Nom;
            }
            return result;
        }

        public static bool Update(long id, string code, string nom)
        {
            var model = new FamilleModel { Id = id, Code = code, Nom = nom };
            var repository = new FamilleRepository();
            return repository.Update(model);
        }
    }

    public partial class FamilleItem
    {
        public bool DuplicateError { get; set; }

        public bool IsValid { get; set; }

        public string Rang { get; set; }

        public void Copy(FamilleModel model)
        {
            Id = model.Id;
            Code = model.Code;
            Nom = model.Nom;
            DuplicateError = model.DuplicateError;
            Rang = model.Rang;
        }

        public void HasError()
        {
            if (Code == null)
            {
                IsValid = false;
            }
            if (Nom == null)
            {
                IsValid = false;
            }
        }
    }

    public class FamilleModel
    {
        public FamilleModel()
        {
            //
        }

        public FamilleModel(FamilleItem famille)
        {
            Copy(famille);
        }

        public string Code { get; set; }

        public bool DuplicateError { get; set; }

        public long Id { get; set; }

        public bool IsValid { get; set; }

        public string Nom { get; set; }

        public string Rang { get; set; }

        public void Copy(FamilleItem item)
        {
            Id = item.Id;
            Code = item.Code;
            Nom = item.Nom;
            DuplicateError = item.DuplicateError;
            IsValid = item.IsValid;
            Rang = item.Rang;
        }
    }

    public class FamilleRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public void Add(FamilleItem famille)
        {
            _context.FamilleItems.InsertOnSubmit(famille);
        }

        public int Count()
        {
            return _context.FamilleItems.Count();
        }

        public bool Create(FamilleModel model)
        {
            var famille = new FamilleItem();
            model.Id = Count() + 1;
            famille.Copy(model);
            Add(famille);
            try
            {
                _context.SubmitChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Delete(FamilleItem famille)
        {
            _context.FamilleItems.DeleteOnSubmit(famille);
        }

        public IQueryable<FamilleItem> Familles()
        {
            return _context.FamilleItems;
        }

        public FamilleItem Get(long id)
        {
            return _context.FamilleItems.SingleOrDefault(d => d.Id == id);
        }

        public FamilleItem GetByName(string name)
        {
            return _context.FamilleItems.SingleOrDefault(d => d.Nom == name);
        }

        public bool Save()
        {
            try
            {
                _context.SubmitChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Update(FamilleModel model)
        {
            FamilleItem item = Get(model.Id);
            item.Copy(model);
            try
            {
                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class TrapRepository
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public Detail2 Get(long id)
        {
            return _context.Detail2s.SingleOrDefault(d => d.Id == id);
        }
    }
}