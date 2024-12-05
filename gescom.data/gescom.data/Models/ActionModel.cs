﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gescom.data.Models
{
    public static class ActionHelpers
    {
        public static void Cancel(long id)
        {
            List<ExitItem> liste = CancelExits(id);
            CancelReturn(liste);
        }

        public static List<ExitItem> CancelExits(long id)
        {
            var detailCart = new DetailCart();
            var detailRept = new DetailRepository();
            foreach (DetailItem detail in detailCart.Where(detail => detail.Rang == id))
            {
                detailRept.Cancel(detail.Id);
                detailRept.Save();
            }
            var moveCart = new MoveCart();
            var moveRept = new MoveRepository();
            foreach (MoveItem move in moveCart.Moves)
            {
                if (move.Rang == id)
                {
                    MoveItem mItem = moveRept.Get(move.Id);
                    mItem.Quantite = 0;
                    moveRept.Save();
                    CancelTaxeAndBonus(move.Id);
                }
            }
            var cart = new ExitCart();
            var result = new List<ExitItem>();
            DiaryItem diary = DiaryHelpers.Get(id);
            long pid = -1;
            if (diary.Pid != null) pid = (long)diary.Pid;
            var exitReptory = new ExitRepository();
            foreach (ExitItem item in cart.Where(item => item.Rang == id))
            {
                item.Pid = pid;
                result.Add(item);
                ExitItem item1 = exitReptory.Get(item.Id);
                item1.Groupe = 8;
                item1.Quantite = 0;
                exitReptory.Save();
            }
            return result;
        }

        public static void CancelReturn(List<ExitItem> liste)
        {
            if (liste.Count == 0)
            {
                return;
            }
            long pid = liste[0].Pid;
            var model = new DiaryModel(10, pid);
            DiaryItem diary = DiaryHelpers.Create(model);
            foreach (ExitItem item in liste)
            {
                item.Rang = diary.Id;
                item.Groupe = 10;
                float quantite = 0;
                if (item.Quantite != null) quantite = (float)item.Quantite;
                long numero = 0;
                if (item.Numero != null) numero = (long)item.Numero;
                ArticleHelpers.Add(numero, quantite);
            }
            var entryRept = new EntryRepository();
            var moveRept = new MoveRepository();
            foreach (ExitItem item in liste)
            {
                entryRept.Add(item);
                entryRept.Save();
                moveRept.Add(item);
                moveRept.Save();
            }
        }

        public static void CreateBonus(MoveItem item)
        {
            var reptory = new BonusRepository();
            if (item.Prix != null)
            {
                var prix = (float)item.Prix;
                item.Prix = StdCalcul.GetSimpleLisseF(prix);
            }
            reptory.Add(item);
        }

        public static DeclItem CreateDecl(MoveItem item)
        {
            var reptory = new DeclRepository();
            if (item.Prix != null)
            {
                var prix = (float)item.Prix;
                item.Prix = prix;
            }
            DeclItem result = reptory.Add(item);
            return result;
        }

        public static void CreateDetails(ActionModel model)
        {
            var repository = new DetailRepository();
            repository.Create(model);
        }

        public static long DoActions(List<ElementModel> elements, long index)
        {
            long i = -1 * index;
            long result = 0;
            switch (i)
            {
                case 3:
                    result = DoErrCount(elements);
                    break;

                case 4:
                    result = DoDefect(elements);
                    break;

                case 5:
                    result = DoSteal(elements);
                    break;
            }
            return result;
        }

        public static long DoAutoCommande(List<ElementModel> elements)
        {
            var diary = new DiaryModel(6);
            DiaryHelpers.Create(diary);
            float total = elements.Sum(item1 => ToCommand(item1, diary));
            var model = new CashModel(diary, -1, total);
            return CashHelpers.Create(model);
        }

        public static long DoBuy(IEnumerable<ElementModel> elements, PersonItem item)
        {
            var diary = new DiaryModel(0, item.Id);
            DiaryHelpers.Create(diary);
            float total = 0;
            float taxe = PriceHelpers.GetTaxe() / 100;
            foreach (ElementModel elementModel in elements)
            {
                ToAdd(elementModel, diary);
                float prix = PriceHelpers.GetPAchat(elementModel.Id);
                if (elementModel.Taxable == 0)
                {
                    total += prix * elementModel.Quantite;
                }
                if (elementModel.Taxable == 1)
                {
                    total += (1 + taxe) * prix * elementModel.Quantite;
                }
                ProdHelpers.RealizedCommande(elementModel.Id, elementModel.Quantite);
            }
            var model = new CashModel(diary, item, total) { Groupe = 0 };
            return CashHelpers.Create(model);
        }

        public static long DoBuy2(ElementModel elementModel)
        {
            var diary = new DiaryModel(0, elementModel.Pid);
            DiaryHelpers.Create(diary);
            float total = 0;
            float taxe = PriceHelpers.GetTaxe() / 100;
            ToAdd(elementModel, diary);
            float prix = PriceHelpers.GetPAchat(elementModel.Id);
            if (elementModel.Taxable == 0)
            {
                total += prix * elementModel.Quantite;
            }
            if (elementModel.Taxable == 1)
            {
                total += (1 + taxe) * prix * elementModel.Quantite;
            }
            ProdHelpers.RealizedCommande(elementModel.Id, elementModel.Quantite);
            var model = new CashModel(diary, elementModel.Pid, total) { Groupe = 0 };
            return CashHelpers.Create(model);
        }

        public static long DoCmde(List<ElementModel> elements, PersonItem item)
        {
            var diary = new DiaryModel(6, item.Id);
            DiaryHelpers.Create(diary);
            float total = elements.Sum(item1 => ToCommand(item1, diary));
            var model = new CashModel(diary, item, total);
            return CashHelpers.Create(model);
        }

        public static long DoDefect(List<ElementModel> elements)
        {
            var diary = new DiaryModel(4);
            DiaryHelpers.Create(diary);
            float total = elements.Sum(item => ToDefect(item, diary));
            var model = new CashModel(diary, total);
            return CashHelpers.Create(model);
        }

        public static long DoErrCount(List<ElementModel> elements)
        {
            var diary = new DiaryModel(3);
            DiaryHelpers.Create(diary);
            float total = elements.Sum(item => ToDefect(item, diary));
            var model = new CashModel(diary, total);
            return CashHelpers.Create(model);
        }

        public static long DoReturn(List<ElementModel> elements)
        {
            var diary = new DiaryModel(1, -1);
            DiaryHelpers.Create(diary);
            float total = elements.Sum(item => ToAdd(item, diary));
            var model = new CashModel(diary, total);
            return CashHelpers.Create(model);
        }

        public static CashModel DoSell(List<ElementModel> elements, PersonItem item)
        {
            var diary = new DiaryModel(2, item.Id);
            var c = DiaryHelpers.Create(diary);
            float total = elements.Sum(item1 => ToSell(item1, diary));
            var model = new CashModel(diary, item, total) { Groupe = 2 };
            CashHelpers.Create(model);
            model.Id = c.Id;
            return model;
        }

        public static long DoSimulate(List<ElementModel> elements, PersonItem item)
        {
            var diary = new DiaryModel(9, item.Id);
            DiaryItem diary1 = DiaryHelpers.Create(diary);
            float total = elements.Sum(item1 => ToSimulate(item1, diary));
            var model = new CashModel(diary, item, total);
            CashHelpers.CreateNoBox(model);
            return diary1.Id;
        }

        public static long DoSteal(List<ElementModel> elements)
        {
            var diary = new DiaryModel(5);
            DiaryHelpers.Create(diary);
            float total = elements.Sum(item => ToDefect(item, diary));
            var model = new CashModel(diary, total);
            return CashHelpers.Create(model);
        }

        public static IEnumerable<Detail2> FilterByDate(IEnumerable<Detail2> liste, DateTime debut, DateTime fin)
        {
            debut = debut.AddDays(-1);
            return liste.Where(item => (item.DATY <= fin) && (item.DATY >= debut));
        }

        public static float GetAmountedBonus(long id)
        {
            return
                GetBonusById(id)
                    .Where(item => (item.Prix != null) && (item.Quantite != null))
                    .Sum(item => (float)(item.Prix * item.Quantite));
        }

        public static IEnumerable<BonusItem> GetBonusById(long id)
        {
            var cart = new BonusCart();
            return cart.Bonus.Where(item => item.Rang == id);
        }

        public static IEnumerable<DetailItem> GetDatedItems(long id, DateTime debut, DateTime fin)
        {
            debut = debut.AddDays(-1);
            return GetDetails(id).Where(item => (item.Daty <= fin) && (item.Daty >= debut));
        }

        public static IEnumerable<DetailText> GetDatedTexts(long id, DateTime debut, DateTime fin)
        {
            var result = new List<DetailText>();
            foreach (DetailItem item in GetDatedItems(id, debut, fin))
            {
                var text = new DetailText();
                text.Copy(item);
                result.Add(text);
            }
            return result;
        }

        public static IEnumerable<DetailItem> GetDetails(long id)
        {
            return GetDetails().Where(item => (item.Numero == id) && (item.Groupe >= 0));
        }

        public static IEnumerable<DetailItem> GetDetails()
        {
            var repo = new DetailRepository();
            return repo.Details();
        }

        public static IEnumerable<Detail2> GetDetails2(long id)
        {
            return GetDetails2().Where(item => item.Numero == id);
        }

        public static IEnumerable<DetailMag> GetEntrees(long id)
        {
            var context = new DataGescomDataContext();
            return context.DetailMags.Where(item => item.Numero == id);
        }

        public static IEnumerable<Detail2> GetDetails2()
        {
            var context = new DataGescomDataContext();
            return context.Detail2s;
        }

        public static List<ElementModel> GetElements(IEnumerable<OperationItem> liste)
        {
            var elements = new List<ElementModel>();
            foreach (OperationItem item in liste)
            {
                var element = new ElementModel();
                element.Copy(item);
                elements.Add(element);
            }
            return elements;
        }

        public static List<ElementModel> GetElements(IEnumerable<OperationAuto> liste)
        {
            var elements = new List<ElementModel>();
            foreach (OperationAuto item in liste)
            {
                var element = new ElementModel();
                element.Copy(item);
                elements.Add(element);
            }
            return elements;
        }

        public static ActionType GetSetActionType(long groupe)
        {
            var actype = new ActionType(groupe);
            return actype;
        }

        public static IEnumerable<DetailText> GetTexts(long id)
        {
            var result = new List<DetailText>();
            foreach (DetailItem item in GetDetails(id))
            {
                var text = new DetailText();
                text.Copy(item);
                var d = DiaryHelpers.Get(item.Id);
                text.Tache = d.Tache;
                text.Operateur = d.Nom;
                var da = d.Datum;
                if (da != null)
                {
                    text.Daty = (DateTime)da;
                }
                result.Add(text);
            }
            return result;
        }

        private static void CancelTaxeAndBonus(long id)
        {
            var declRept = new DeclRepository();
            declRept.Cancel(id);
            var bonusRept = new BonusRepository();
            bonusRept.Cancel(id);
        }

        private static float ToAdd(ElementModel element, DiaryModel diary)
        {
            element.Vid = diary.Pid;
            var model = new ActionModel(diary);
            var repository = new EntryRepository();
            return repository.Add(model, element);
        }

        private static float ToCommand(ElementModel element, DiaryModel diary)
        {
            var model = new ActionModel(diary);
            var repository = new EntryRepository();
            return repository.Command(model, element);
        }

        private static float ToDefect(ElementModel element, DiaryModel diary)
        {
            var model = new ActionModel(diary);
            var repository = new ExitRepository();
            return repository.RemoveDefect(model, element);
        }

        private static float ToSell(ElementModel element, DiaryModel diary)
        {
            element.Cid = diary.Pid;
            element.Groupe = 2;
            var model = new ActionModel(diary) { Groupe = 2, Wid = element.Wid };
            element.Pid = diary.Pid;
            var repository = new ExitRepository();
            return repository.Remove(model, element);
        }

        private static float ToSimulate(ElementModel element, DiaryModel diary)
        {
            element.Cid = diary.Pid;
            element.Groupe = 9;
            var model = new ActionModel(diary) { Groupe = 9 };
            element.Pid = diary.Pid;
            var repository = new ExitRepository();
            return repository.Simulate(model, element);
        }
    }

    public class ActionModel
    {
        public ActionModel()
        {
            //
        }

        public ActionModel(DiaryModel model)
        {
            Rang = model.Id;
            Groupe = model.Groupe;
        }

        public long Forme { get; set; }

        public long Groupe { get; set; }

        public long Id { get; set; }

        public string Info { get; set; }

        public bool IsValid { get; set; }

        public long Numero { get; set; }

        public string Operateur { get; set; }

        public long Pid { get; set; }

        public float Prix { get; set; }

        public float QEntre { get; set; }

        public float QSort { get; set; }

        public float Quantite { get; set; }

        public long Rang { get; set; }

        public float Rx { get; set; }

        public int Taxable { get; set; }

        public long Wid { get; set; }

        public void Copy(ElementModel element)
        {
            Prix = element.Prix;
            Quantite = element.Quantite;
            Numero = element.Id;
            Rx = element.Rx;
            Forme = element.Forme;
            Taxable = element.Taxable;
        }
    }

    public class ActionType
    {
        public ActionType()
        {
            Init();
        }

        public ActionType(long groupe)
        {
            Set(groupe);
        }

        public long Groupe { get; set; }
        public string Tache { get; set; }
        public string Titre { get; set; }

        private void Init()
        {
            Tache = "EN";
            Titre = "ENTREE";
        }

        private void Set(long groupe)
        {
            Groupe = groupe;
            switch (Groupe)
            {
                case 1:
                    Tache = "AV";
                    Titre = "AVOIR";
                    break;

                case 2:
                    Tache = "VT";
                    Titre = "VENTE";
                    break;

                case 3:
                    Tache = "RE";
                    Titre = "REBUT ERREUR";
                    break;

                case 4:
                    Tache = "RD";
                    Titre = "REBUT DEFECTION";
                    break;

                case 5:
                    Tache = "RV";
                    Titre = "REBUT VOL";
                    break;

                case 6:
                    Tache = "CO";
                    Titre = "COMMANDE";
                    break;

                case 7:
                    Tache = "CP";
                    Titre = "COMPTAGE";
                    break;

                case 8:
                    Tache = "AN";
                    Titre = "ANNULATION";
                    break;

                case 9:
                    Tache = "DP";
                    Titre = "DEMANDE PRIX";
                    break;

                case 10:
                    Tache = "NA";
                    Titre = "RETOUR ANNULATION";
                    break;

                case 11:
                    Tache = "RG";
                    Titre = "PAIEMENT";
                    break;

                case 17:
                    Tache = "RP";
                    Titre = "PAIE BONUS";
                    break;

                default:
                    Tache = "EN";
                    Titre = "ENTREE";
                    break;
            }
        }
    }

    public class BonusCart : IEnumerable<BonusItem>
    {
        public BonusCart()
        {
            Bonus = new List<BonusItem>();
            var repository = new BonusRepository();
            int count = repository.Count();
            if (count == 0)
            {
                return;
            }
            Parallel.ForEach(repository.Bonus(), item => Bonus.Add(item));
        }

        public List<BonusItem> Bonus { get; set; }

        public IEnumerator<BonusItem> GetEnumerator()
        {
            return Bonus.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public partial class BonusItem
    {
        public bool DuplicateError { get; set; }

        public bool IsValid { get; set; }

        public float Montant { get; set; }

        public long Pid { get; set; }

        public float Rx { get; set; }

        public int Taxable { get; set; }

        public void Copy(ActionModel model)
        {
            Id = model.Id;
            Rang = model.Rang;
            Numero = model.Numero;
            Prix = model.Prix;
            Quantite = model.Quantite;
            Groupe = model.Groupe;
            Pid = model.Pid;
            Rx = model.Rx;
            Taxable = model.Taxable;
        }

        public void Copy(ExitItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Numero = item.Numero;
            Prix = item.Prix;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Taxable = item.Taxable;
        }

        public void Copy(MoveItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Numero = item.Wid;
            Wid = item.Wid;
            Prix = item.Rx;
            Pid = item.Wid;
            Taxable = item.Taxable;
        }
    }

    public class BonusRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public void Add(BonusItem item)
        {
            _context.BonusItems.InsertOnSubmit(item);
            Save();
        }

        public void Add(MoveItem item)
        {
            if (item == null)
            {
                return;
            }
            if (item.Groupe != 2)
            {
                return;
            }
            var element = new BonusItem();
            element.Copy(item);
            Add(element);
        }

        public IQueryable<BonusItem> Bonus()
        {
            return _context.BonusItems;
        }

        public void Cancel(long id)
        {
            BonusItem item = Get(id);
            if (item == null)
            {
                return;
            }
            item.Quantite = 0;
            Save();
        }

        public int Count()
        {
            return _context.BonusItems.Count();
        }

        public void Delete(BonusItem item)
        {
            _context.BonusItems.DeleteOnSubmit(item);
        }

        public BonusItem Get(long id)
        {
            return _context.BonusItems.SingleOrDefault(d => d.Id == id);
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
    }

    public class DeclCart : IEnumerable<DeclItem>
    {
        public DeclCart()
        {
            Declares = new List<DeclItem>();
            var repository = new DeclRepository();
            int count = repository.Count();
            if (count == 0)
            {
                return;
            }
            foreach (DeclItem element in repository.Decls())
            {
                var item = new DeclItem();
                item.Copy(element);
                Declares.Add(item);
            }
        }

        public List<DeclItem> Declares { get; set; }

        public IEnumerator<DeclItem> GetEnumerator()
        {
            return Declares.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public partial class DeclItem
    {
        public bool DuplicateError { get; set; }

        public bool IsValid { get; set; }

        public float Montant { get; set; }

        public long Pid { get; set; }

        public float Rx { get; set; }

        public int Taxable { get; set; }

        public long Wid { get; set; }

        public void Copy(DeclItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Numero = item.Numero;
            Prix = item.Prix;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Wid = item.Wid;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }

        public void Copy(ActionModel model)
        {
            Id = model.Id;
            Rang = model.Rang;
            Numero = model.Numero;
            Prix = model.Prix;
            Quantite = model.Quantite;
            Groupe = model.Groupe;
            Pid = model.Pid;
            Rx = model.Rx;
            Forme = model.Forme;
            Taxable = model.Taxable;
        }

        public void Copy(ExitItem item)
        {
            Id = item.Id;
            if (item.Rang != null) Rang = (long)item.Rang;
            if (item.Numero != null) Numero = (long)item.Numero;
            if (item.Prix != null) Prix = (float)item.Prix;
            if (item.Quantite != null) Quantite = (float)item.Quantite;
            if (item.Groupe != null) Groupe = (long)item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Taxable = item.Taxable;
        }

        public void Copy(MoveItem item)
        {
            Id = item.Id;
            if (item.Rang != null) Rang = (long)item.Rang;
            if (item.Numero != null) Numero = (long)item.Numero;
            if (item.Prix != null) Prix = (float)item.Prix;
            if (item.Quantite != null) Quantite = (float)item.Quantite;
            if (item.Groupe != null) Groupe = (long)item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }

        public void Copy(EntryItem item)
        {
            Id = item.Id;
            if (item.Rang != null) Rang = (long)item.Rang;
            if (item.Numero != null) Numero = (long)item.Numero;
            if (item.Prix != null) Prix = (float)item.Prix;
            if (item.Quantite != null) Quantite = (float)item.Quantite;
            if (item.Groupe != null) Groupe = (long)item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }
    }

    public class DeclRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public bool Add(DeclItem item)
        {
            try
            {
                _context.DeclItems.InsertOnSubmit(item);
                Save();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public DeclItem Add(MoveItem item)
        {
            var element = new DeclItem();
            if (item == null)
            {
                return null;
            }
            element.Copy(item);
            float prix = 0;
            if (item.Groupe == 0)
            {
                prix = PriceHelpers.GetPAchat(element.Numero);
                float taux = PriceHelpers.GetTaxe() / 100;
                if (element.Taxable == 1)
                {
                    prix = prix * taux;
                }
                else
                {
                    prix = 0;
                }
                element.Prix = prix;
                //element.Prix = StdCalcul.GetSimpleLisseF(prix);
                Add(element);
                return element;
            }
            if ((element.Groupe != 1) && (element.Groupe != 2) && (element.Groupe != 8)) return null;
            if (item.Prix != null) prix = (float)item.Prix;
            element.Prix = 0;
            if (element.Taxable == 1)
            {
                prix = PriceHelpers.CalculReboursTva(prix);
                element.Prix = prix;
                // element.Prix = StdCalcul.GetSimpleLisseF(prix);
            }
            Add(element);
            return element;
        }

        public void Cancel(long id)
        {
            DeclItem item = Get(id);
            if (item == null)
            {
                return;
            }
            item.Quantite = 0;
            Save();
        }

        public int Count()
        {
            return _context.DeclItems.Count();
        }

        public IQueryable<DeclItem> Decls()
        {
            return _context.DeclItems;
        }

        public void Delete(long id)
        {
            DeclItem item = Get(id);
            Delete(item);
            Save();
        }

        public void Delete(DeclItem item)
        {
            _context.DeclItems.DeleteOnSubmit(item);
        }

        public DeclItem Get(long id)
        {
            return _context.DeclItems.SingleOrDefault(d => d.Id == id);
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
    }

    public class DetailCart : IEnumerable<DetailItem>
    {
        public DetailCart()
        {
            Details = new List<DetailItem>();
            var repository = new DetailRepository();
            int count = repository.Count();
            if (count == 0)
            {
                return;
            }
            foreach (DetailItem element in repository.Details())
            {
                DetailItem item = element;
                if (element.Rang != null)
                {
                    var id = (long)element.Rang;
                    DiaryModel day = DiaryHelpers.GetReference(id);
                    item.Daty = day.Datum;
                    item.Tache = day.Tache;
                    item.Operateur = day.Nom;
                    item.Info = item.Daty + "-" + item.Operateur;
                    SetMontant(item);
                }
                Details.Add(item);
            }
        }

        public List<DetailItem> Details { get; set; }

        public IEnumerator<DetailItem> GetEnumerator()
        {
            return Details.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void SetMontant(DetailItem item)
        {
            if (item == null)
            {
                return;
            }
            if (item.Prix == null)
            {
                return;
            }
            var prix = (float)item.Prix;
            float q1 = 0;
            float q2 = 0;
            if (item.QEntre != null)
            {
                q1 = (float)item.QEntre;
            }
            if (item.QSort != null)
            {
                q2 = (float)item.QSort;
            }
            item.Quantite = q1 + q2;
            item.Montant = prix * item.Quantite;
            if (q1 <= 0)
            {
                item.QEntre = null;
            }
            if (q2 <= 0)
            {
                item.QSort = null;
            }
        }
    }

    public partial class DetailItem
    {
        public DateTime Daty { get; set; }

        public bool DuplicateError { get; set; }

        public long Forme { get; set; }

        public string Info { get; set; }

        public bool IsValid { get; set; }

        public float Montant { get; set; }

        public string Operateur { get; set; }

        public long Pid { get; set; }

        public float Quantite { get; set; }

        public float Rx { get; set; }

        public string Tache { get; set; }

        public int Taxable { get; set; }

        public long Wid { get; set; }

        public void Copy(ActionModel model)
        {
            Id = model.Id;
            Rang = model.Rang;
            Numero = model.Numero;
            Prix = model.Prix;
            Groupe = model.Groupe;
            QEntre = model.QEntre;
            QSort = model.QSort;
            Info = model.Info;
            Pid = model.Pid;
            Rx = model.Rx;
            Forme = model.Forme;
            Taxable = model.Taxable;
        }
    }

    public class DetailRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public void Add(DetailItem item)
        {
            _context.DetailItems.InsertOnSubmit(item);
        }

        public void Cancel(long id)
        {
            DetailItem detail = Get(id);
            detail.QSort = null;
            detail.Groupe = -1;
            detail.QEntre = null;
            Save();
        }

        public int Count()
        {
            return _context.DetailItems.Count();
        }

        public void Create(ActionModel model)
        {
            if (model.Groupe < 0)
            {
                return;
            }
            if (model.Groupe > 5)
            {
                return;
            }
            var item = new DetailItem();
            item.Copy(model);
            item.Id = Count() + 1;
            if ((item.Groupe >= 0) && (item.Groupe < 2))
            {
                item.QEntre = model.Quantite;
                Add(item);
                _context.SubmitChanges();
                return;
            }
            item.QSort = model.Quantite;
            Add(item);
            _context.SubmitChanges();
        }

        public void Delete(long id)
        {
            DetailItem item = Get(id);
            Delete(item);
            Save();
        }

        public void Delete(DetailItem item)
        {
            _context.DetailItems.DeleteOnSubmit(item);
        }

        public IQueryable<DetailItem> Details()
        {
            return _context.DetailItems;
        }

        public DetailItem Get(long id)
        {
            return _context.DetailItems.SingleOrDefault(d => d.Id == id);
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
    }

    public class DetailText
    {
        public DateTime Daty { get; set; }

        public bool DuplicateError { get; set; }

        public long Groupe { get; set; }

        public string Id { get; set; }

        public string Info { get; set; }

        public bool IsValid { get; set; }

        public string Montant { get; set; }

        public string Numero { get; set; }

        public string Operateur { get; set; }

        public long Pid { get; set; }

        public string Prix { get; set; }

        public string QEntre { get; set; }

        public string QSort { get; set; }

        public string Quantite { get; set; }

        public string Rang { get; set; }

        public string Tache { get; set; }

        public string Vid { get; set; }

        public void Copy(ActionModel model)
        {
            Id = StdCalcul.DoubleToSpaceFormat(model.Id);
            Rang = StdCalcul.DoubleToSpaceFormat(model.Rang);
            Numero = StdCalcul.DoubleToSpaceFormat(model.Numero);
            Prix = StdCalcul.DoubleToSpaceFormat(model.Prix);
            Groupe = model.Groupe;
            QEntre = StdCalcul.DoubleToSpaceFormat(model.QEntre);
            QSort = StdCalcul.DoubleToSpaceFormat(model.QSort);
            Operateur = model.Operateur;
            Info = model.Info;
            Pid = model.Pid;
        }

        public void Copy(DetailItem item)
        {
            Daty = item.Daty;
            Tache = item.Tache;
            Id = StdCalcul.DoubleToSpaceFormat(item.Id);
            if (item.Rang != null)
            {
                var r = (double)item.Rang;
                Rang = StdCalcul.DoubleToSpaceFormat(r);
            }
            if (item.Numero != null)
            {
                var n = (double)item.Numero;
                Numero = StdCalcul.DoubleToSpaceFormat(n);
            }
            if (item.Prix != null)
            {
                var p = (double)item.Prix;
                Prix = StdCalcul.DoubleToSpaceFormat(p);
            }
            if (item.QEntre != null)
            {
                var e = (double)item.QEntre;
                QEntre = StdCalcul.DoubleToSpaceFormat(e);
            }
            if (item.QSort != null)
            {
                var q = (double)item.QSort;
                QSort = StdCalcul.DoubleToSpaceFormat(q);
            }
            Montant = StdCalcul.DoubleToSpaceFormat(item.Montant);
            Operateur = item.Operateur;
            Info = item.Info;
            Pid = item.Pid;
        }
    }

    public class EntryCart : IEnumerable<EntryItem>
    {
        public EntryCart()
        {
            Entrees = new List<EntryItem>();
            var repository = new EntryRepository();
            int count = repository.Count();
            if (count == 0)
            {
                return;
            }
            foreach (EntryItem element in repository.Entrees())
            {
                EntryItem item = element;
                Entrees.Add(item);
            }
        }

        public List<EntryItem> Entrees { get; set; }

        public IEnumerator<EntryItem> GetEnumerator()
        {
            return Entrees.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public partial class EntryItem
    {
        public bool DuplicateError { get; set; }

        public bool IsValid { get; set; }

        public long Pid { get; set; }

        public float Rx { get; set; }

        public int Taxable { get; set; }

        public long Wid { get; set; }

        public void Copy(ActionModel model)
        {
            Id = model.Id;
            Rang = model.Rang;
            Numero = model.Numero;
            Prix = model.Prix;
            Quantite = model.Quantite;
            Groupe = model.Groupe;
            Pid = model.Pid;
            Rx = model.Rx;
            Forme = model.Forme;
            Taxable = model.Taxable;
        }

        public void Copy(ExitItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Numero = item.Numero;
            Prix = item.Prix;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }
    }

    public class EntryRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public float Add(ActionModel model, ElementModel element)
        {
            // crée l'entrée.
            element.Prix = PriceHelpers.GetPAchat(element.Id);
            // re init commande.
            ProdHelpers.NewArrive(element.Id);
            Create(model, element);
            // augmente le stock de l'identifiant.
            ArticleHelpers.Add(element.Id, element.Quantite);
            // mise à jour de la date d'achat.
            if (element.Pid >= 0)
            {
                EventsHelper.UpdateOnBuy(element.Id, element.Vid);
            }
            // crée le mouvement.
            var moveRepository = new MoveRepository();
            moveRepository.Create(model, element);
            float montant = element.Prix * element.Quantite;
            return montant;
        }

        public void Add(ExitItem item)
        {
            if (item.Numero != null)
            {
                var numero = (long)item.Numero;
                var element = new EntryItem();
                element.Copy(item);
                element.Prix = PriceHelpers.GetPRevient(numero);
                element.Id = Count() + 1;
                Add(element);
            }
        }

        public void Add(EntryItem item)
        {
            _context.EntryItems.InsertOnSubmit(item);
        }

        public float Command(ActionModel model, ElementModel element)
        {
            element.Prix = ProdHelpers.GetPrice(element.Id);
            Create(model, element);
            var moveRepository = new MoveRepository();
            moveRepository.Create(model, element);
            float montant = element.Prix * element.Quantite;
            return montant;
        }

        public int Count()
        {
            return _context.EntryItems.Count();
        }

        public float Count(ActionModel model, ElementModel element)
        {
            element.Prix = PriceHelpers.GetPDetail(element.Id);
            // crée le mouvement.
            var moveRepository = new MoveRepository();
            moveRepository.Create(model, element);
            float montant = element.Prix * element.Quantite;
            return montant;
        }

        public void Delete(EntryItem item)
        {
            _context.EntryItems.DeleteOnSubmit(item);
        }

        public IQueryable<EntryItem> Entrees()
        {
            return _context.EntryItems;
        }

        public EntryItem Get(long id)
        {
            return _context.EntryItems.SingleOrDefault(d => d.Id == id);
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

        private void Create(ActionModel model, ElementModel element)
        {
            var item = new EntryItem();
            model.Copy(element);
            item.Copy(model);
            item.Id = Count() + 1;
            Add(item);
            _context.SubmitChanges();
            ActionHelpers.CreateDetails(model);
        }
    }

    public class ExitCart : IEnumerable<ExitItem>
    {
        public ExitCart()
        {
            Sorties = new List<ExitItem>();
            var repository = new ExitRepository();
            int count = repository.Count();
            if (count == 0)
            {
                return;
            }
            foreach (ExitItem element in repository.Sorties())
            {
                ExitItem item = element;
                Sorties.Add(item);
            }
        }

        public List<ExitItem> Sorties { get; set; }

        public IEnumerator<ExitItem> GetEnumerator()
        {
            return Sorties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public partial class ExitItem
    {
        public bool DuplicateError { get; set; }

        public bool IsValid { get; set; }

        public long Pid { get; set; }

        public float Rx { get; set; }

        public int Taxable { get; set; }

        public long Wid { get; set; }

        public void Copy(ActionModel model)
        {
            Id = model.Id;
            Rang = model.Rang;
            Numero = model.Numero;
            Prix = model.Prix;
            Quantite = model.Quantite;
            Groupe = model.Groupe;
            Pid = model.Pid;
            Rx = model.Rx;
            Forme = model.Forme;
            Taxable = model.Taxable;
        }

        public void Copy(EntryItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Numero = item.Numero;
            Prix = item.Prix;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }
    }

    public class ExitRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public void Add(ExitItem item)
        {
            _context.ExitItems.InsertOnSubmit(item);
        }

        public void Cancel(long id)
        {
            ExitItem item = Get(id);
            item.Quantite = 0;
            _context.SubmitChanges();
        }

        public int Count()
        {
            return _context.ExitItems.Count();
        }

        public void Delete(ExitItem item)
        {
            _context.ExitItems.DeleteOnSubmit(item);
        }

        public ExitItem Get(long id)
        {
            return _context.ExitItems.SingleOrDefault(d => d.Id == id);
        }

        // pour la vente seulement.
        public float Remove(ActionModel model, ElementModel element)
        {
            model.Groupe = 2;
            element.Groupe = 2;
            element.Prix = GetPrice(element.Id, element.Pid);
            // diminue le stock de l'identifiant.
            ArticleHelpers.Remove(element.Id, element.Quantite);
            // mise à jour date vente.
            EventsHelper.UpdateOnSold(element.Id, element.Cid);
            // crée le mouvement.
            float quantite = element.Quantite;
            float prix = element.Prix;
            var moveRepository = new MoveRepository();
            element.Quantite = quantite;
            moveRepository.Create(model, element);
            // crée la sortie
            element.Prix = PriceHelpers.CalculReboursHt(element.Id, prix);
            element.Prix = StdCalcul.GetSimpleLisseF(element.Prix);
            Create(model, element);
            return quantite * prix;
        }

        // pour les rebuts seulement.
        public float RemoveDefect(ActionModel model, ElementModel element)
        {
            // diminue le stock de l'identifiant.
            element.Prix = PriceHelpers.GetPRevient(element.Id);
            ArticleHelpers.Remove(element.Id, element.Quantite);
            // crée la sortie
            Create(model, element);
            // crée le mouvement.
            var moveRepository = new MoveRepository();
            float quantite = element.Quantite;
            element.Quantite = quantite;
            moveRepository.Create(model, element);
            return quantite * element.Prix;
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

        // demande prix.
        public float Simulate(ActionModel model, ElementModel element)
        {
            model.Groupe = 9;
            element.Groupe = 9;
            element.Prix = GetPrice(element.Id, element.Pid);
            Create(model, element);
            var moveRepository = new MoveRepository();
            float quantite = element.Quantite;
            element.Quantite = quantite;
            moveRepository.Create(model, element);
            return quantite * element.Prix;
        }

        public IQueryable<ExitItem> Sorties()
        {
            return _context.ExitItems;
        }

        private void Create(ActionModel model, ElementModel element)
        {
            var item = new ExitItem();
            model.Copy(element);
            item.Copy(model);
            item.Id = Count() + 1;
            Add(item);
            _context.SubmitChanges();
            ActionHelpers.CreateDetails(model);
        }

        private float GetPrice(long id, long pid)
        {
            float result;
            long groupe = PersonHelpers.GetGroup(pid);
            switch (groupe)
            {
                case 2:
                    result = PriceHelpers.GetPDetail(id);
                    break;

                case 3:
                    result = PriceHelpers.GetPSpecial(id);
                    break;

                case 4:
                    result = PriceHelpers.GetPExtra(id);
                    break;

                default:
                    result = PriceHelpers.GetPGros(id);
                    break;
            }
            return result;
        }
    }

    public class MoveCart : IEnumerable<MoveItem>
    {
        public MoveCart()
        {
            Moves = new List<MoveItem>();
            var repository = new MoveRepository();
            int count = repository.Count();
            if (count == 0)
            {
                return;
            }
            foreach (MoveItem element in repository.Moves())
            {
                float p = 0;
                if (element.Prix != null) p = (float)element.Prix;
                float q = 0;
                if (element.Quantite != null) q = (float)element.Quantite;
                element.Montant = p * q;
                MoveItem item = element;
                Moves.Add(item);
            }
        }

        public List<MoveItem> Moves { get; set; }

        public IEnumerator<MoveItem> GetEnumerator()
        {
            return Moves.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public partial class MoveItem
    {
        public bool DuplicateError { get; set; }

        public bool IsValid { get; set; }

        public float Montant { get; set; }

        public long Pid { get; set; }

        public float Rx { get; set; }

        public int Taxable { get; set; }

        public long Wid { get; set; }

        public void Copy(ActionModel model)
        {
            Id = model.Id;
            Rang = model.Rang;
            Numero = model.Numero;
            Prix = model.Prix;
            Quantite = model.Quantite;
            Groupe = model.Groupe;
            Pid = model.Pid;
            Wid = model.Wid;
            Rx = model.Rx;
            Forme = model.Forme;
            Taxable = model.Taxable;
        }

        public void Copy(ExitItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Numero = item.Numero;
            Prix = item.Prix;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }

        public void Copy(EntryItem item)
        {
            Id = item.Id;
            Rang = item.Rang;
            Numero = item.Numero;
            Prix = item.Prix;
            Quantite = item.Quantite;
            Groupe = item.Groupe;
            Pid = item.Pid;
            Rx = item.Rx;
            Forme = item.Forme;
            Taxable = item.Taxable;
        }
    }

    public class MoveRepository : IData
    {
        private readonly DataGescomDataContext _context = new DataGescomDataContext();

        public void Add(MoveItem item)
        {
            _context.MoveItems.InsertOnSubmit(item);
        }

        public void Add(ExitItem item)
        {
            if (item.Numero != null)
            {
                var numero = (long)item.Numero;
                var element = new MoveItem();
                element.Copy(item);
                element.Prix = PriceHelpers.GetPRevient(numero);
                element.Id = Count() + 1;
                Add(element);
            }
        }

        public void Cancel(long id)
        {
            MoveItem item = Get(id);
            if (item == null)
            {
                return;
            }
            item.Quantite = 0;
            Save();
        }

        public int Count()
        {
            return _context.MoveItems.Count();
        }

        public void Create(ActionModel model, ElementModel element)
        {
            var item = new MoveItem();
            model.Copy(element);
            item.Copy(model);
            item.Id = Count() + 1;
            Add(item);
            _context.SubmitChanges();
            ActionHelpers.CreateDecl(item);
            ActionHelpers.CreateBonus(item);
        }

        public void Delete(long id)
        {
            MoveItem item = Get(id);
            Delete(item);
            Save();
        }

        public void Delete(MoveItem item)
        {
            _context.MoveItems.DeleteOnSubmit(item);
        }

        public MoveItem Get(long id)
        {
            return _context.MoveItems.SingleOrDefault(d => d.Id == id);
        }

        public IQueryable<MoveItem> Moves()
        {
            return _context.MoveItems;
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
    }
}