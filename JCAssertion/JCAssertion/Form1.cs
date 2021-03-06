﻿// License
//
//
//  JCAssertion : Un ensemble d’outils
//              pour configurer et vérifier les environnements 
//              de tests sous windows.
//
//  Copyright 2016-2019 Jean-Claude Parent 
// 
//  Informations : www.jcassertion.org
//
// Ce fichier fait partie de JCAssertion.
//
// JCAssertion  est un logiciel libre ; vous pouvez le redistribuer ou le 
// modifier suivant les termes de la GNU General Public License telle que 
// publiée par la Free Software Foundation ; soit la version 2 de la 
// licence, soit (à votre gré) toute version ultérieure.
// 
// JCAssertion est distribué dans l'espoir qu'il sera utile, 
// mais SANS AUCUNE GARANTIE ; sans même la garantie tacite 
// de QUALITÉ MARCHANDE ou d'ADÉQUATION à UN BUT PARTICULIER. 
// Consultez la GNU General Public License pour plus de détails.
// 
// Vous devez avoir reçu une copie de la GNU General Public License 
// en même temps que JCAssertion ; si ce n'est pas le cas, consultez
// <http://www.gnu.org/licenses>. Selon la recommandation  de la fondation 
// le seul texte officiel est celui en anglais car la fondation ne peut garantir
// que la traduction dans une langue  assurera les mêmes protections.
// 
// License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JCAssertionCore;
using System.IO;
using System.Xml;
using System.Timers;
using System.Threading;
using JCAMC;


namespace JCAssertion
{
    public partial class JCAssertion : Form
    {
        

        // Variables modifiées juste par le thread secondaire si il est actif
        public static Boolean Interactif = false;
        public static Boolean Avertir = false;
        public static String Message = "";
        static int NombreReussi = 0;
        static int NombreEchec = 0;
        static int NombreCas = 0;
        static String JournalActivite = null;
        public static Exception ExceptionGlobale = new Exception();
        public static Boolean ExceptionRencontree = false;
        private static int CodeDeRetour = 97;
        public static String MessageEchec = "";
        private static Boolean UnitTest = false;

        



        
        
        // Variables en lecture seule par le thread secondaire

        public static  Boolean AnnulerExecution = false ;
        public string[] args = new string[0];
        public static Int32  EchecsMaximum = 0 ;
        
        

        // Variables dans chaque thread
        Boolean Init = false;
        String Usage = Environment.NewLine  +   "usage :" + Environment.NewLine + Environment.NewLine +
            "JCAssertion /FA:fichierassertion /fv:fichierdevariables "+
            "/j:FichierDeJournal.txt /av /i";
        
        JCAVariable mesArguments;
        JCAConsole maConsole;
        JCAUtilitaires Utilitaire;
        JCACore monJCACore;
        System.Threading.Thread monThread; 
        System.Timers.Timer monTimer = new System.Timers.Timer();
        // Boolean Debug = true;
        
        public void InitJCAssertion()
            // l'appel aux constructeurs complexes a été mis
            // ici pour pouvoir faire un try catch
        {
          if (! Init)
              {
            try {
                Utilitaire = new JCAUtilitaires();
                Init = true ;
                mesArguments = new JCAssertionCore.JCAVariable();
                maConsole = new JCAssertionCore.JCAConsole();
                monJCACore = new JCACore();

                }
            catch (Exception excep)
                {
                    ExceptionGlobale = excep ;
                    ExceptionRencontree = true;
                    CodeDeRetour = 99;
                    throw;
                }
              } // ! init
            
        }


        public void setUnitTest()
        {
            UnitTest = true;
        }

        public int getCodeDeRetour()
            {
                return CodeDeRetour;
            }
        
        public Exception  getException()
        {
            return ExceptionGlobale;
        }

        public void setInteractif(Boolean Valeur)
        {
            Interactif = Valeur;
        }

        public Boolean monThreadIsAlive()
        {
            return monThread.IsAlive; 
        }

        public String  getMessage()
        {
            return Message ;
        }

        private void Informer(String Texte, Boolean Severe = false )
        {
            AjouteActivite(Texte);
            if (Severe)
                {
                    Message = Environment.NewLine + Texte;
                    if ((Interactif) || (Avertir))
                        BackgroundThreadMessageBox( this  , Texte);
                      
                }
        }

        private void VerifieFini(object source, ElapsedEventArgs e)
        {
            
            if (! monThread.IsAlive)
            {
                monTimer.Stop();
                Console.WriteLine(Message);
                if(ExceptionRencontree )
                    {
                        Console.WriteLine("Erreur:" + ExceptionGlobale.GetType()+
                            Environment.NewLine + ExceptionGlobale.Message   ); 
                    }
                if(! Interactif && ! (Avertir && ExceptionRencontree ))
                    Environment.Exit(CodeDeRetour )  ;
                
            }
        }

        private DialogResult BackgroundThreadMessageBox(
            IWin32Window owner, 
            string text)
        {

            if (this.InvokeRequired)
            {
                return (DialogResult)this.Invoke(new
          Func<DialogResult>(
                                       () => 
                                       { return MessageBox.Show(owner, text); }));

            }
            else
            {
                return MessageBox.Show(owner, text);
            }
        }

        public delegate void NAJtbxFAssertionCallBack(String Texte);

        private void NAJtbxFAssertion(String Texte)
          {
            if (tbxFAssertion.InvokeRequired)
                {
                    NAJtbxFAssertionCallBack CB =
                        new NAJtbxFAssertionCallBack(NAJtbxFAssertion);
                    this.Invoke(CB, new Object[] { Texte });
                } else 
                {
                    tbxFAssertion.Text = Texte;
                    tbxFAssertion.Refresh(); 
                }
          }

        public delegate void MAJtbxFVariablesCallBack(String Texte);

        private void MAJtbxFVariables(String Texte)
        {
            if (tbxFVariables.InvokeRequired)
            {
                MAJtbxFVariablesCallBack CB =
                    new MAJtbxFVariablesCallBack(MAJtbxFVariables);
                this.Invoke(CB, new Object[] { Texte });
            }
            else
            {
                tbxFVariables.Text = Texte;
                tbxFVariables.Refresh();
            }
        }

        public delegate void AjouteActiviteCallBack(String Texte);
        
        private void AjouteActivite(String Texte )
        {
            if (tbxActivite.InvokeRequired)
             {
                 AjouteActiviteCallBack CB = new 
                     AjouteActiviteCallBack(AjouteActivite);
                this.Invoke(CB, new Object[] { Texte } );
                
             }
            else 
             {
                tbxActivite.Text = tbxActivite.Text + 
                    Environment.NewLine + Texte;
                tbxActivite.Refresh(); 
            if (JournalActivite != null )
            {
               StreamWriter fileJournal =  File.AppendText(JournalActivite  );
               fileJournal.WriteLine(Environment.NewLine + Texte );
               fileJournal.Close();
            } 
           }   
        }

        public void  ExecuteAssertion()
        {
            

            // ici c'est ce qui va s'exécuter dans le thread secondaire
            try {

                 
                CodeDeRetour = Execute();
                } catch (Exception excep)
                {
                    Console.WriteLine("Erreur : " + excep.Message);
                     
                    Message = "ERREUR==>" + excep.GetType() +
                        Environment.NewLine   + excep.Message;
                    Utilitaire.EventLogErreur(Message );
 
                    Console.WriteLine(Message); 
                    Informer (Message, Avertir );
                    ExceptionGlobale = excep;
                    ExceptionRencontree = true;
                    CodeDeRetour = 99;
                }
        }

        // Methode utilisé  par le load et qui peutêtreunittestée
        public int Execute()
        {
            InitJCAssertion();
            NombreCas = 0;
            NombreEchec = 0;
            NombreReussi = 0;

            Message = "Démarrage";
            if ((args.Length == 0) && (! UnitTest ) ) Interactif = true;
            mesArguments = maConsole.Arguments(args);

            if ((mesArguments.GetValeurVariable("AV") != null) &&
                (mesArguments.GetValeurVariable("AV") != ""))
            {
                Avertir = true;
            }

            if ((mesArguments.GetValeurVariable("I") != null) &&
                (mesArguments.GetValeurVariable("I") != ""))
            {
                Interactif  = true;
            }

            if (!JCAUtilitaires.EVSourceExiste())
                Informer(Environment.NewLine +
                    "Avertissement : La source de journal d'événement 'JCAssertion' " +
                    "doit être définie pour avoir accès à "+
                    "tous les messages d'erreurs. Consultez www.jcassertion.org Configuration requise " +
                    "^pour plus de détails.");


            // l'argument d0 provoque une exceptions

            if ((mesArguments.GetValeurVariable("D0") != null) &&
                (mesArguments.GetValeurVariable("D0") != ""))
            {
                throw new Exception(
                    "Exception déclenchée volontairement par l'argument /D0 ");
            }

            if ((mesArguments.GetValeurVariable("MAX") != null) &&
                (mesArguments.GetValeurVariable("MAX") != ""))
            {
                EchecsMaximum = JCAMiniCore.ConvertirSansErreur(
                    mesArguments.GetValeurVariable("MAX"));
                
            }
           
            // Vérifier qu'au moins le nom de fichier d'assertion est fourni
            
            if ((mesArguments.GetValeurVariable("FA") == null) ||
                (mesArguments.GetValeurVariable("FA") == ""))
                    {
                        Informer("Ce programme doit recevoir des arguments enligne de commande." + Usage, true) ;
                        return 99;
                   }
            // Valider un peu les arguments
            String FichierAssertion = mesArguments.GetValeurVariable("FA");
            String FichierVariable = "";
            if ((mesArguments.GetValeurVariable("FV") != null) &&
                (mesArguments.GetValeurVariable("FV") != "")) 
                    FichierVariable = mesArguments.GetValeurVariable("FV");
            
            if (!System.IO.File.Exists(FichierAssertion))
            {
                Informer( "Le fichier d'assertion . " +
                    FichierAssertion + " n'existe pas." , true );
                return 99;
            }

           if((FichierVariable != "" ) &&
               (! System.IO.File.Exists (FichierVariable)))
           {
                Informer  ("Le fichier de variables . " +
                    FichierVariable + " n'existe pas.", true );
                return 99;
            }

            
            if ((mesArguments.GetValeurVariable("J") != null) &&
               (mesArguments.GetValeurVariable("J") != ""))
               // Désactiver la journalisation  du core
               // Le^programme va faire un  journal plus complet
               monJCACore.Journaliser = false ;
               JournalActivite =    mesArguments.GetValeurVariable("J");
               if (System.IO.File.Exists(JournalActivite))
                   System.IO.File.Delete(JournalActivite);   

            //
            // Journaliser qelques ifo utiles
               Informer("JCAssertion version " +
                   JCAssertionCore.JCACore.Constantes.Version);
               Informer("Date de l'exécution : " +
                   DateTime.Now.ToLongDateString() + " " +  
                   DateTime.Now.ToLongTimeString());
            
            // Informer des config intéressantes

               if (EchecsMaximum > 0)
                   Informer("Arrêter d'évaluer les assertions après "+
                       EchecsMaximum.ToString()+
                       " échecs"); 
            // commencer le traitementproprement dit
            NAJtbxFAssertion(FichierAssertion);
            MAJtbxFVariables(FichierVariable) ;
            Informer("Lecture du fichier d'assertion : " 
                + FichierAssertion );
            monJCACore.Load(FichierAssertion );
            NombreCas = monJCACore.NombreCas;
            Informer("Nombre de cas à traiter : " + monJCACore.NombreCas.ToString () );
            if(FichierVariable != "")
            {
                Informer ("Lecture du fichier de variables : "
                + FichierVariable);
                monJCACore.Variables.LireFichier(FichierVariable );
            }

            int i = 1;
            foreach (XmlNode monCas in monJCACore.getListeDeCas())
                {
                    if (AnnulerExecution) break;
                    Informer ("Exécution du cas " + i.ToString() );
                    if (monJCACore.ExecuteCas(monCas))
                        {
                            NombreReussi = NombreReussi + 1;
                            Informer ("Assertion vraie") ;
                            Informer (monJCACore.Message);
                        }
                    else
                        {
                            MessageEchec = monJCACore.MessageEchec  ;    
                            Informer ("Assertion fausse");
                            Informer (monJCACore.Message);
                            Informer("Détail de l'échec de l'assertion" +
                                Environment.NewLine +
                                "----");
                            Informer(MessageEchec,Avertir );
                            Informer("-----" +
                                Environment.NewLine +
                                "Fin du détail de l'échec de l'assertion");
                            NombreEchec = NombreEchec + 1;
                            if ((EchecsMaximum != 0) &&
                                (NombreEchec >= EchecsMaximum ))
                                {
                                    Informer(Environment.NewLine+
                                        NombreEchec.ToString() +
                                        " assertion(s) ont échouées ce qui dépasse le maximum d'échecs spécifié, l'évaluation des assertions a été arrêtée."+
                                        Environment.NewLine  );
                                    break;
                                }
                        }
                    i = i + 1;
                }
            Informer("Fin de l'exécution");
            Informer("Cas réussis : " + NombreReussi.ToString() + " sur " + NombreCas.ToString()  );
            Informer("Cas en échec : " + NombreEchec.ToString() + 
                " sur " + NombreCas.ToString());
            if((NombreEchec + NombreReussi) != NombreCas)
                Informer("Cas non évalués : " + 
                    (NombreCas -  (NombreEchec + NombreReussi )).ToString() + 
                " sur " + NombreCas.ToString());

            if(NombreEchec > 0) 
                return 1;
            else
              return 0;
        }

        public void LancerThread()
            {
                monThread = new System.Threading.Thread(
                    new System.Threading.ThreadStart(ExecuteAssertion));
                monThread.Start();
                while (! monThread.IsAlive  )
                    Thread.Sleep(1);
  

                
            }

        public JCAssertion()
        {
            InitializeComponent();
        }

        private void JCAssertion_Load(object sender, EventArgs e)
        {
            
            try {
                InitJCAssertion (); 
                FormClosed += new FormClosedEventHandler(Form1_FormClosing);

                LancerThread();
                // configurer et lancer le timer
                monTimer.AutoReset = true;
                monTimer.Interval = 1000;
                monTimer.Elapsed += new ElapsedEventHandler(VerifieFini);
                monTimer.Start(); 
                // initialiser le handler de fermeture de la fenetre

 

             } catch (Exception excep)
                 {
                     Console.WriteLine("Erreur : " + excep.Message );
                     Informer(excep.Message, true );
                     ExceptionRencontree = true;
                     ExceptionGlobale = excep;
                     CodeDeRetour = 99;
                    if(! Interactif ) 
                        Environment.Exit(99);
                }

            // end run
            


        }

        
        private void Form1_FormClosing(object sender, EventArgs e)
        {
            if (monThread.IsAlive  )
                {
                    Informer("Le programme a été fermé par l'utilisateur"); 
                    monThread.Abort();
                    Environment.Exit(77);  
                } else
                {
                    Environment.Exit(CodeDeRetour ); 
                };

        }

        private void tbxActivite_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Action à faire quand l'utilisateur 
        /// clique sur le bouton annuler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            Informer("Programme annulé par le boutonannuler");
            AnnulerExecution = true;
        }

        /// <summary>
        /// Popup de "à propos de JCAsseertion"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show  ("JCAssertion "+
                JCAssertionCore.JCACore.Constantes.Version +
                Environment.NewLine +
                "(C)opyright 2016-2019 Jean-Claude Parent" +
                Environment.NewLine +
                "http://www.jcassertion.org");

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
