﻿using DevExpress.XtraEditors;
using gescom.create.Models;
using gescom.data.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace gescom.create.Views
{
    public partial class XtraRegBonus : XtraForm
    {
        private readonly ArticleRepository _articleRepository;
        private readonly DistItem _distItem;
        private readonly OperationModel _operation;
        private readonly long _prodId;
        private readonly TarifModel _tarifModel;
        private ActeModel _acte;
        private int _actePlace = -1;
        private int _acteVerif = -1;
        private ArticleItem _articleItem;
        private ProdCheck _commande;
        private long _distNumPlace;
        private DuoItem _duo;
        private bool _etatEntre;
        private bool _etatPrix;
        private long _forme;
        private long _t;
        private IEnumerable<VendorItem> _vendors;
        private VoirModel _voir;
        private long _x = -2;
        private long _y = -2;

        //
        public XtraRegBonus()
        {
            InitializeComponent();
        }

        public XtraRegBonus(long id)
        {
            InitializeComponent();
            _prodId = id;
            _vendors = PersonHelpers.GetVendorsList();
            _articleRepository = new ArticleRepository();
            _duo = new DuoItem();
            _tarifModel = new TarifModel();
            _articleItem = new ArticleItem();
            _distItem = new DistItem();
            _distItem = DistHelpers.Get(id);
            _operation = new OperationModel();
            _operation = OperationHelpers.Get(id);
            _commande = ProdHelpers.GetCommande(id);
            _acte = new ActeModel(ActeHelpers.Get(id));
            _voir = new VoirModel(VoirHelpers.Get(id));
            _etatPrix = _voir.VoirPrix;
            _etatEntre = _voir.VoirEntre;
            DistInit();
            ArticleInit(id);
            ProdInit();
            OperInit();
            CheckInit();
            CheckState(id);
        }

        public long ActicleId { get; set; }

        public bool ArticleIsValid { get; set; }

        public bool ArticleVerifier()
        {
            if (string.IsNullOrEmpty(nom.Text) || string.IsNullOrEmpty(refce.Text) ||
                string.IsNullOrEmpty(plancher.Text) ||
                string.IsNullOrEmpty(prix.Text))
            {
                return false;
            }
            pdetail.PositiveColor = Color.Black;
            if ((coeff.Value <= 0) || (mgros.Value <= 0))
            {
                return false;
            }
            double prAch;
            try
            {
                prAch = double.Parse(prix.Text);
            }
            catch (Exception)
            {
                return false;
            }
            var rate = StdCalcul.DecimalToDouble(coeff.Value);
            var prRevient = StdCalcul.GetPrixComplete(prAch, rate);
            prRevient = StdCalcul.GetSimpleLisse(prRevient);
            revient.Text = StdCalcul.AfficherPrix(prRevient);
            if (prRevient <= 0)
            {
                return false;
            }
            rate = StdCalcul.DecimalToDouble(mgros.Value);
            var prGros = StdCalcul.GetPrixComplete(prRevient, rate);
            if (mgros.Value == 0)
            {
                prGros = StdCalcul.GetPrixLisse(prGros) + 100;
            }
            rate = StdCalcul.DecimalToDouble(mSpecial.Value);
            var prSpec = StdCalcul.GetPrixComplete(prRevient, rate);
            if (mSpecial.Value == 0)
            {
                prSpec = StdCalcul.GetPrixLisse(prSpec) + 100;
            }
            rate = StdCalcul.DecimalToDouble(mExtra.Value);
            var prExtra = StdCalcul.GetPrixComplete(prRevient, rate);
            if (mExtra.Value == 0)
            {
                prExtra = StdCalcul.GetPrixLisse(prExtra) + 100;
            }
            rate = StdCalcul.DecimalToDouble(mdetail.Value);
            var prDetail = StdCalcul.GetPrixComplete(prRevient, rate);
            if (mdetail.Value == 0)
            {
                prDetail = StdCalcul.GetPrixLisse(prDetail) + 100;
            }
            var prime = prDetail - prRevient;
            double myprime = 0;
            if (prime > 0)
            {
                var primerate = StdCalcul.DecimalToFloat(txPrime.Value) / 100;
                myprime = primerate * prime;
                myprime = StdCalcul.GetPrixLisse(myprime);
            }
            double taux = PriceHelpers.GetTaxe() / 100;
            if (taxable.Checked)
            {
                prGros = (1 + taux) * prGros;
                prDetail = (1 + taux) * prDetail;
                prSpec = (1 + taux) * prSpec;
                prExtra = (1 + taux) * prExtra;
            }
            prGros = StdCalcul.GetPrixLisse(prGros);
            prSpec = StdCalcul.GetPrixLisse(prSpec);
            prExtra = StdCalcul.GetPrixLisse(prExtra);
            prDetail = StdCalcul.GetPrixLisse(prDetail);
            if (prGros > prDetail)
            {
                pdetail.PositiveColor = Color.Crimson;
                return false;
            }
            pdetail.Text = StdCalcul.AfficherPrix(prDetail);
            pspecial.Text = StdCalcul.AfficherPrix(prSpec);
            pgros.Text = StdCalcul.AfficherPrix(prGros);
            vprime.Text = StdCalcul.AfficherPrix(myprime);
            pextra.Text = StdCalcul.AfficherPrix(prExtra);
            return true;
        }

        protected bool ArticleCreer()
        {
            if (!ArticleIsValid)
            {
                return false;
            }
            if (ActicleId == 0)
            {
                return false;
            }
            var primeRate = StdCalcul.DecimalToFloat(txPrime.Value);
            _tarifModel.SetMarge(_tarifModel.Id, float.Parse(prix.Text), coeff.Value, mgros.Value, mdetail.Value,
                mSpecial.Value,
                mExtra.Value, taxable.Checked, primeRate);
            _tarifModel.SafeUpdate();
            _articleItem.Nom = nom.Text;
            _articleItem.Refce = refce.Text;
            _articleItem.Catalogue = katalog.Text;
            _articleItem.Ig = StdCalcul.GetCategoryId(family.Text);
            _articleItem.Iu = StdCalcul.GetUnityId(unity.Text);
            _articleItem.Taxable = taxable.Checked ? 1 : 0;
            _articleItem.Forme = formel.Checked ? 1 : 0;
            if (!string.IsNullOrEmpty(plancher.Text))
            {
                _articleItem.Seuil = float.Parse(plancher.Text);
            }
            var otherId = DataHelpers.GetArticleIdByName(nom.Text);
            if ((_tarifModel.Id != otherId) && (otherId > 0))
            {
                ErrorHelpers.ShowErrorDuplicate();
                nom.Focus();
                nom.SelectAll();
                return false;
            }
            return Coder();
        }

        protected bool DistCreer()
        {
            Coder();
            if (string.IsNullOrEmpty(distRemark.Text))
            {
                distRemark.Text = "0";
            }
            if (string.IsNullOrEmpty(distQte.Text))
            {
                distQte.Text = "0";
            }
            _distItem.Nom = distNom.Text;
            _distItem.Quantite = float.Parse(distQte.Text);
            _distItem.Description = b2.Text;
            //_distItem.Description = distObs.Text;
            _distItem.Numero = PlaceHelpers.GetId(distNewPlace.Text);
            if (_distItem.Numero == null) return false;
            var n = (long)_distItem.Numero;
            var place = PlaceHelpers.Get(n);
            if (place.IsReserved != null)
            {
                var i = (long)place.IsReserved;
                if (i == 1)
                {
                    _distItem.Numero = _distNumPlace;
                }
            }
            PriceHelpers.UpdateUniteNameTaxable(_distItem.Id, prodUnity.Text);
            PlaceHelpers.Fill(n);
            ArticleHelpers.PutDescription(_distItem.Id, distRemark.Text);
            return DistHelpers.Update(_distItem);
        }

        protected void DistInit()
        {
            var article = ArticleHelpers.Get(_distItem.Id);
            _duo = ArticleHelpers.GetDuo(_distItem.Id);
            CheckVisibility();
            reponse.Text = _duo.S2;
            distRemark.Text = article.Description;
            b2.Text = _distItem.Description;
            //distObs.Text = _distItem.Description;
            distNom.Text = _distItem.Nom;
            distQte.Text = _distItem.Quantite.ToString();
            if (_distItem.Numero == null) return;
            var id = (long)_distItem.Numero;
            _distNumPlace = id;
            var place = PlaceHelpers.GetIndex(id);
            distPlace.Text = place.Code;
            distNewPlace.Text = place.Code;
            secondPlace.Text = place.Nom;
            foreach (var variable in PlaceHelpers.GetList())
            {
                distPlaceNew.Items.Add(variable);
            }
            distPlaceNew.Text = place.Code;
        }

        protected bool ProdSave()
        {
            var model = new ProdModel { Id = _prodId };
            var test = false;
            if (string.IsNullOrEmpty(prodNom.Text))
            {
                ErrorHelpers.ShowError("Le nom ne peut être vide.");
                prodNom.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(prodQte.Text))
            {
                ErrorHelpers.ShowError("La quantité ne peut être nulle.");
                prodQte.Focus();
                return false;
            }
            else
            {
                test = StdCalcul.DoubleConversion(prodQte.Text);
                if (!test)
                {
                    ErrorHelpers.ShowError("Valeur incorrecte.");
                    prodQte.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(prodPrix.Text))
            {
                ErrorHelpers.ShowError("Le prix ne peut être nul.");
                prodPrix.Focus();
                return false;
            }
            else
            {
                test = StdCalcul.DoubleConversion(prodPrix.Text);
                if (!test)
                {
                    ErrorHelpers.ShowError("Valeur incorrecte.");
                    prodPrix.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(prodVend.Text))
            {
                ErrorHelpers.ShowError("Le fournisseur de préférence doit être renseigné.");
                prodVend.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(QcomEdit.Text))
            {
                ErrorHelpers.ShowError("La quantité à commander ne peut être nulle.");
                QcomEdit.Focus();
                return false;
            }
            else
            {
                test = StdCalcul.DoubleConversion(QcomEdit.Text);
                if (!test)
                {
                    ErrorHelpers.ShowError("Valeur incorrecte.");
                    QcomEdit.Focus();
                    return false;
                }
                else
                {
                    _acte.Qcder = double.Parse(QcomEdit.Text);
                }
            }
            model.Description = prodDesk.Text;
            model.Rq = prodRq.Text;
            model.Nom = prodNom.Text;
            model.Refce = prodRefce.Text;
            model.Quantite = float.Parse(prodQte.Text);
            model.Prix = float.Parse(prodPrix.Text);
            model.Iu = StdCalcul.GetUnityId(prodUnity.Text);
            model.Pid = StdCalcul.GetVendorId(prodVend.Text);
            if (chkCommande.Checked)
            {
                _voir.CheckCommande = true;
                model.Drapeau = -1;
            }
            else
            {
                _voir.CheckCommande = false;
            }
            ProdHelpers.Update(model);
            return true;
        }

        private void Alerter()
        {
            var dispo = 0;
            try
            {
                dispo = int.Parse(cumDispo.Text);
            }
            catch (Exception)
            {
                dispo = 0;
            }
            var qcom = int.Parse(distQte.Text);
            _voir.AlertCom = false;
            if ((qcom > 0) && (qcom >= dispo))
            {
                _voir.AlertCom = true;
            }
            var qach = int.Parse(QcomEdit.Text);
            _voir.AlertAch = false;
            if ((qach > 0) && (qach >= dispo))
            {
                _voir.AlertAch = true;
            }
            var qs = int.Parse(plancher.Text);
            _voir.AlertVte = false;
            if ((qs > 0) && (qs >= dispo))
            {
                _voir.AlertVte = true;
            }
        }

        private void ActeSave()
        {
            // state.
            if (normeBtn.Checked) { _acte.State = ""; }
            if (dangerBtn.Checked) { _acte.State = "Danger"; }
            if (questionBtn.Checked) { _acte.State = "Question"; }
            if (superBtn.Checked) { _acte.State = "Super"; }
            //
            var p = 2;
            var v = 2;
            if (placeCours.Checked) { p = -1; _acte.Placer = -1; _voir.VoirPlace = true; }
            if (placeNorme.Checked) { p = 0; }
            if (verifCours.Checked) { v = -1; _acte.Verif = -1; _voir.VoirVerif = true; }
            if (verifNorme.Checked) { v = 0; }
            if (_acteVerif > 0) { _acte.Verif = _acteVerif; _voir.VoirVerif = false; }
            else
            {
                if (v < 2)
                {
                    _acte.Verif = v;
                    _voir.VoirVerif = false;
                    if (v < 0)
                    {
                        _voir.VoirVerif = true;
                    }
                }
            }
            if (_actePlace > 0) { _acte.Placer = _actePlace; _voir.VoirPlace = false; }
            else
            {
                if (p < 2)
                {
                    _acte.Placer = p;
                    _voir.VoirPlace = false;
                    if (p < 0)
                    {
                        _voir.VoirPlace = true;
                    }
                }
            }
            _voir.VoirEntre = _etatEntre;
            _voir.VoirPrix = _etatPrix;
            if (_etatEntre)
            {
                _acte.Entrer = 1;
            }
            if (_etatPrix)
            {
                _acte.Priter = 1;
            }
            if (prixCours.Checked)
            {
                _acte.Priter = -1;
                _voir.VoirPrix = true;
            }
            if (entreCours.Checked)
            {
                _acte.Entrer = -1;
                _voir.VoirEntre = true;
            }
            if (prixNorme.Checked)
            {
                _acte.Priter = 0;
                _voir.VoirPrix = false;
            }
            if (entreNorme.Checked)
            {
                _acte.Entrer = 0;
                _voir.VoirEntre = false;
            }
            _voir.CheckCommande = false;
            if (chkCommande.Checked)
            {
                _voir.CheckCommande = true;
            }
            _acte.Qcder = double.Parse(QcomEdit.Text);
            Alerter();
            ActeHelpers.Update(_acte);
            VoirHelpers.Update(_voir);
        }

        private void annuler_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ArticleDisable()
        {
            cadre.Enabled = false;
            tarifBox.Enabled = false;
            ArticleIsValid = false;
        }

        private void ArticleInit(long id)
        {
            ActicleId = id;
            ArticleIsValid = true;
            if (id <= 0)
            {
                ArticleDisable();
                ErrorHelpers.ShowError("ERREUR GENERAL.");
                return;
            }
            labelNums.Visible = true;
            codex.Visible = true;
            labiNom.Visible = true;
            leNom.Visible = true;
            _articleItem = _articleRepository.Get(id);
            if (_articleItem.Ig == null)
            {
                return;
            }
            var index = -1;
            var count = 0;
            var i = (long)_articleItem.Ig;
            var name = FamilleHelpers.GetName(i);
            var fcode = FamilleHelpers.GetCode(i).Substring(0, 1) + id.ToString();
            foreach (var variable in FamilleHelpers.GetListFamilles())
            {
                family.Items.Add(variable);
                count++;
                if (variable == name)
                {
                    index = count;
                }
            }
            family.SelectedIndex = index - 1;
            if (_articleItem.Iu == null)
            {
                return;
            }
            i = (long)_articleItem.Iu;
            name = UniteHelpers.GetName(i);
            index = -1;
            count = 0;
            foreach (var variable in UniteHelpers.GetListUnites())
            {
                unity.Items.Add(variable);
                count++;
                if (variable == name)
                {
                    index = count;
                }
            }
            if (_articleItem.Forme == 1)
            {
                formel.Checked = true;
            }
            unity.SelectedIndex = index - 1;
            nombre.Text = _articleItem.Id.ToString(CultureInfo.InvariantCulture);
            codage.Text = fcode;
            textBox1.Text = fcode;
            leNom.Text = _articleItem.Nom;
            nom.Text = _articleItem.Nom;
            Text = _articleItem.Nom;
            refce.Text = _articleItem.Refce;
            katalog.Text = _articleItem.Catalogue;
            taxable.Checked = _articleItem.Taxable == 1;
            plancher.Text = _articleItem.Seuil.ToString();
            _tarifModel.ReadFast(id);
            prix.Text = Math.Round(_tarifModel.Exonere.Achat).ToString(CultureInfo.InvariantCulture);
            coeff.Value = StdCalcul.DoubleToDecimal(_tarifModel.Marge.Revient);
            mgros.Value = StdCalcul.DoubleToDecimal(_tarifModel.Marge.Gros);
            mdetail.Value = StdCalcul.DoubleToDecimal(_tarifModel.Marge.Detail);
            mSpecial.Value = StdCalcul.DoubleToDecimal(_tarifModel.Marge.Special);
            mExtra.Value = StdCalcul.DoubleToDecimal(_tarifModel.Marge.Extra);
            txPrime.Value = StdCalcul.DoubleToDecimal(_tarifModel.Prime.Achat);
            ArticleVerifier();
        }

        private void CheckInit()
        {
            chkArret.Checked = _commande.Arret;
            chkEpuise.Checked = _commande.Epuise;
            chkCommande.Checked = _commande.CheckCommande;
            if ((_commande.Arret) || ((_commande.Epuise)))
            {
                return;
            }
        }

        private void CheckState(long id)
        {
            var _actePlace = _acte.Placer;
            var _acteVerif = _acte.Verif;
            string state = _acte.State;
            QcomEdit.Text = _acte.Qcder.ToString();
            // placement.
            if (_actePlace == 0) { placeNorme.Checked = true; }
            if (_actePlace < 0) { placeCours.Checked = true; }
            if (_actePlace > 0) { placeFini.Checked = true; }
            // verification.
            if (_acteVerif == 0) { verifNorme.Checked = true; }
            if (_acteVerif < 0) { verifCours.Checked = true; }
            if (_acteVerif > 0) { verifFini.Checked = true; }
            // state.            
            if (state == "Danger") { dangerBtn.Checked = true; }
            if (state == "Question") { questionBtn.Checked = true; }
            if (state == "Super") { superBtn.Checked = true; }
            // finition enter and price.
            if (_acte.Entrer > 0) { entreFini.Checked = true; }
            if (_acte.Priter > 0) { prixFini.Checked = true; }
            //
            chkCommande.Checked = false;
            if (_voir.CheckCommande)
            {
                chkCommande.Checked = true;
            }
            if (_voir.VoirPrior)
            {
                checkPrior.Checked = true;
            }
            else
            {
                checkPrior.Checked = false;
            }
            if (_voir.VoirEntre)
            {
                entreCours.Checked = true;
            }
            if (_voir.VoirPrix)
            {
                prixCours.Checked = true;
            }
            rienButton.Checked = _commande.Etat0;
        }

        private void CheckVisibility()
        {
            if (_duo.L != null)
            {
                long z = (long)_duo.L;
                if (z < 1) { radioHaute.Checked = true; }
                else
                {
                    radioMoyen.Checked = true;
                }
            }
            else
            {
                radioBas.Checked = true;
            }
        }

        private bool Coder()
        {
            string code;
            code = _articleItem.Code + "-I";
            _forme = 0;
            foreach (var p in _vendors)
            {
                if (p.Nom == prodVend.Text)
                {
                    _forme = (long)p.Forme;
                }
            }
            if (_forme == 1)
            {
                if (taxable.Checked)
                {
                    code = _articleItem.Code + "-T";
                }
                else
                {
                    code = _articleItem.Code + "-X";
                }
            }
            _articleItem.CompleteCode = code;
            return _articleRepository.Update();
        }

        private void coeff_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void creer_Click(object sender, EventArgs e)
        {
            bool b = false;
            b = DistCreer();
            if (!b)
            {
                return;
            }
            ArticleVerifier();
            ArticleCreer();
            OperSave();
            DoCommander();
            ActeSave();
            b = ProdSave();
            if (!b)
            {
                return;
            }
            Close();
        }

        private void distPlaceNew_SelectedIndexChanged(object sender, EventArgs e)
        {
            distNewLabel.Visible = true;
            distNewPlace.Text = distPlaceNew.Text;
        }

        private void DoCommander()
        {
            ProdHelpers.Update(_prodId, chkArret.Checked, chkEpuise.Checked, chkCommande.Checked, rienButton.Checked);
        }

        private void family_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void formel_CheckedChanged(object sender, EventArgs e)
        {
            if (taxable.Checked)
            {
                formel.Checked = true;
            }
            if (formel.Checked && !taxable.Checked)
            {
                taxable.Checked = true;
            }
        }

        private void mdetail_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void mExtra_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void mgros_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void mSpecial_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void nom_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void OperInit()
        {
            dateMode.Text = _operation.D1.ToString("f");
            dateReact.Text = _operation.D2.ToString("f");
            cumAchat.Text = _operation.Qachat.ToString("#,#");
            cumVente.Text = _operation.Qvente.ToString("#,#");
            cumDispo.Text = _operation.QStock.ToString("#,#");
            txt1.Text = _operation.T1;
            b1.Text = _operation.B1;
            distObs.Text = _operation.B2;
            // b2.Text = _operation.B2;
            q1.Text = _operation.Q1.ToString("####");
            s1.Text = _operation.S1;
            if (_duo.Faster == 1)
            {
                checkPrior.Checked = true;
            }
            else
            {
                checkPrior.Checked = false;
            }
            if (_duo.AEntre == null)
            {
                // entrée RAS.
                entreNorme.Checked = true;
            }
            else
            {
                _x = _operation.AEntre;
            }
            if (_duo.APrix == null)
            {
                // prix RAS.
                prixNorme.Checked = true;
            }
            else
            {
                _y = _operation.APrix;
            }
            if (_x == 0)
            {
                entreCours.Checked = true;
                // entrée en cours.
            }
            if (_y == 0)
            {
                prixCours.Checked = true;
                // prix en cours.
            }
            if (_x < 0)
            {
                entreNorme.Checked = true;
                //entrée RAS.
            }
            if (_y < 0)
            {
                prixNorme.Checked = true;
                // prix RAS.
            }
            if (_x == 1)
            {
                entreFini.Checked = true;
                // entrée finie.
            }
            if (_y == 1)
            {
                prixFini.Checked = true;
                // prix fini.
            }
        }

        private void OperSave()
        {
            float p;
            try
            {
                p = float.Parse(q1.Text);
            }
            catch (Exception)
            {
                p = 0;
            }
            long x = -1;
            long y = -1;
            _voir.VoirEntre = false;
            _voir.VoirPrix = false;
            if (placeCours.Checked)
            {
                _voir.VoirPlace = true;
            }
            if (placeNorme.Checked)
            {
                _voir.VoirPlace = false;
            }
            if (verifCours.Checked)
            {
                _voir.VoirVerif = true;
            }
            if (verifNorme.Checked)
            {
                _voir.VoirVerif = false;
            }
            if (prixCours.Checked)
            {
                x = 0;
                _voir.VoirPrix = true;
            }
            if (prixNorme.Checked)
            {
                _voir.VoirPrix = false;
            }
            if (entreCours.Checked)
            {
                y = 0;
                _voir.VoirEntre = true;
            }
            if (entreNorme.Checked)
            {
                _voir.VoirEntre = false;
            }
            long? z = null;
            if (radioHaute.Checked)
            {
                z = 0;
            }
            if (radioMoyen.Checked)
            {
                z = 1;
            }
            if (checkPrior.Checked)
            {
                _t = 1;
                _voir.VoirPrior = true;
            }
            else
            {
                _voir.VoirPrior = false;
            }
            ArticleHelpers.UpdateRecto(_prodId, txt1.Text, s1.Text, p, b1.Text, distObs.Text, x, y, z, _t);
        }

        private void panelControl1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
        }

        private void plancher_EditValueChanged(object sender, EventArgs e)
        {
            if (taxable.Checked)
            {
                formel.Checked = true;
            }
            if (formel.Checked && !taxable.Checked)
            {
                taxable.Checked = true;
            }
        }

        private void prix_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void prixBox_Enter(object sender, EventArgs e)
        {
        }

        private void ProdInit()
        {
            var item = ProdHelpers.Get(_prodId);
            prodNom.Text = item.Nom;
            prodRefce.Text = item.Refce;
            prodQte.Text = item.Quantite.ToString();
            prodPrix.Text = item.Prix.ToString();
            prodDesk.Text = item.Description;
            prodRq.Text = item.Rq;
            if (item.Iu == null) return;
            var i = (long)item.Iu;
            var name = UniteHelpers.GetName(i);
            var index = 0;
            var count = 0;
            foreach (var variable in UniteHelpers.GetListUnites())
            {
                prodUnity.Items.Add(variable);
                count++;
                if (variable == name)
                {
                    index = count;
                }
            }
            prodUnity.SelectedIndex = index - 1;
            prodNom.Focus();
            if (item.Pid != null)
            {
                var j = (long)item.Pid;
                var p = PersonHelpers.Get(j);
                name = p.Nom;
                codeF.Text = p.Code;
            }
            index = 0;
            count = 0;
            foreach (var person in _vendors)
            {
                prodVend.Items.Add(person.Nom);
                count++;
                if (person.Nom == name)
                {
                    index = count;
                }
            }
            if (count > 0)
            {
                prodVend.SelectedIndex = index - 1;
            }
        }

        private void refce_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void txPrime_EditValueChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void unity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArticleVerifier();
        }

        private void XtraRegBonus_Load(object sender, EventArgs e)
        {
            nom.Focus();
        }
    }
}