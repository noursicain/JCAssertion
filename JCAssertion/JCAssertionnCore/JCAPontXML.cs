﻿// License
//
//
//  JCAssertion : Un ensemble d’outils
//              pour configurer et vérifier les environnements 
//              de tests sous windows.
//
//  Copyright 2016,2017 Jean-Claude Parent 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using JCAssertionCore;



namespace JCAssertionCore
{
    public partial    class JCAPontXML
    {


        /// <summary>
        /// Retourne le texte d'une balise XML
        /// ou une chaîne vide si on ne trouve pas la balise
        /// </summary>
        /// <param name="monXMLNode">Un document xml ou une autre structure XML qui peuut contenir la balise recherchée</param>
        /// <param name="maBalise">Balise à rechercher</param>
        /// <returns>La valeur de la balise ou ""</returns>
        public static String  ValeurBalise(XmlNode monXMLNode, String maBalise)
        {
            if (monXMLNode == null)
                return "";
            if (monXMLNode[maBalise] == null)
                return "";
            if (monXMLNode[maBalise].InnerText == null)
                return "";
            if (monXMLNode[maBalise].InnerText == "")
                return "";

            return monXMLNode[maBalise].InnerText;
        }

        /// <summary>
        /// Cherche si une balise est présente au moins une fois
        /// dans une structure xml
        /// </summary>
        /// <param name="monXMLNode">Document ou structure xnl où chercher</param>
        /// <param name="maBalise">Nom de la balise à rechercher</param>
        /// <returns>Trouvé ou non</returns>
        public static Boolean  SiBaliseExiste(
            XmlNode monXMLNode, 
            String maBalise)
            {
                if (monXMLNode == null)
                    return false;
                if (monXMLNode[maBalise] == null)
                    return false;
                if (monXMLNode[maBalise].InnerText == null)
                    return false;
                if (monXMLNode[maBalise].InnerText == "")
                    return false;
            
                return true;
            }
        

        /// <summary>
        /// Retourne une exception sauf si
        /// une balise existe et contient quelque chose différent d'une chaîne vide
        /// </summary>
        /// <param name="monXMLNode">Docuememt ou structure xml à valider</param>
        /// <param name="maBalise">Nom de la balise qui doit exister</param>
        public static void ValideBalise(
            XmlNode monXMLNode, 
            String maBalise)
            {
                if (monXMLNode == null)
                    throw new JCAssertionException("Le XML est vide.");
            
                if (monXMLNode[maBalise] == null)
                    throw new JCAssertionException("Le XML ne contient pas la balise " + 
                        maBalise  + "." + monXMLNode.InnerXml);
                if (monXMLNode[maBalise].InnerText == null)
                    throw new JCAssertionException("La balise " +
                        maBalise + " est vide." + monXMLNode.InnerXml);
                if (monXMLNode[maBalise].InnerText == "")
                    throw new JCAssertionException("La balise " +
                        maBalise + " est vide." + monXMLNode.InnerXml);
            }


        public static   bool  JCAFichierExiste(XmlNode monXMLNode,
            ref string   Message,ref  Dictionary<String, String > Variables,
            ref String MessageEchec)
            {
                Message = Message + Environment.NewLine  + "Assertion FichierExiste\n";
                if (monXMLNode == null) throw new JCAssertionException("Le XML est vide.");
                ValideBalise(monXMLNode, "Fichier");
                string NomFichier = monXMLNode["Fichier"].InnerText;
                NomFichier = JCAVariable.SubstituerVariables(NomFichier, Variables);
                Message = Message + Environment.NewLine  +  "Fichier:" + NomFichier  + "\n";
                Boolean Resultat = File.Exists (NomFichier );
                if (!Resultat) Resultat = System.IO.Directory.Exists(NomFichier);  
                if (Resultat) 
                    {
                    Message = Message + Environment.NewLine  +
                        "Le fichier existe.";
                    MessageEchec = "";
                    }
                else
                    {
                    Message = Message + Environment.NewLine  +
                        "Le fichier n'existe pas.";
                    if (ValeurBalise (monXMLNode, "MessageEchec") != "")
                        MessageEchec = ValeurBalise (monXMLNode, "MessageEchec")
                            + " (Fichier : " + NomFichier + ")";
                    else
                        MessageEchec = "Le fichier " + NomFichier + " n'existe pas et il devrait exister.";
                }
                return Resultat;
            }

        public static bool JCASubstituerVariablesFichier(XmlNode monXMLNode, 
            ref string Message, 
            ref  Dictionary<String,String> Variables)
        {
            Message = Message + Environment.NewLine + "Assertion SubstituerVariablesFichier";
            if (monXMLNode == null) 
                throw new JCAssertionException("Le XML est vide.");
            
            // Valider le fichier de modèle

            ValideBalise(monXMLNode, "FichierModele");
            string NomFichierModele = monXMLNode["FichierModele"].InnerText;
            NomFichierModele = JCAVariable.SubstituerVariables(NomFichierModele, Variables);
            Message = Message + Environment.NewLine + "Fichier de modèle:" + NomFichierModele + Environment.NewLine  ;
            if(!File.Exists(NomFichierModele))
                throw new JCAssertionException("Le fichier modèle n'existe pas." + monXMLNode.InnerXml);
            
            // Valider le fichier de sortie
            ValideBalise(monXMLNode, "FichierSortie");
            
            
            string NomFichierSortie = monXMLNode["FichierSortie"].InnerText;
            NomFichierSortie = JCAVariable.SubstituerVariables(NomFichierSortie, Variables);
            Message = Message + Environment.NewLine + "Fichier de sortie:" + NomFichierSortie + Environment.NewLine;
            
            //Valider le fichier de variable
            
            ValideBalise(monXMLNode, "FichierVariables");
            string NomFichierVariables = monXMLNode["FichierVariables"].InnerText;
            NomFichierVariables = JCAVariable.SubstituerVariables(NomFichierVariables, Variables);
            Message = Message + Environment.NewLine + "Fichier de variables :" + NomFichierVariables + Environment.NewLine;
            if (!File.Exists(NomFichierVariables))
                throw new JCAssertionException("Le fichier de variables n'existe pas." + monXMLNode.InnerXml);
            
            
            JCAUtilitaireFichier UF = new JCAUtilitaireFichier ();
            
            UF.SubstituerVariableFichier(NomFichierModele , NomFichierSortie , NomFichierVariables)  ;

            Message = Message + Environment.NewLine
                + "La substitution des variables dans le fichier a réussie.";
            return true ;
        }

        public static bool JCAContenuFichier(XmlNode monXMLNode,
            ref string Message, ref  Dictionary<String, String> Variables,
            ref String  MessageEchec)
        {
            MessageEchec = "";
            Message = Message + Environment.NewLine + "Assertion ContenuFichier";
            if (monXMLNode == null) 
                throw new JCAssertionException("Le XML est vide.");
            ValideBalise(monXMLNode, "Fichier");
            if((!SiBaliseExiste(monXMLNode,"Contient")) && 
                (!SiBaliseExiste(monXMLNode,"NeContientPas")))
                 throw new JCAssertionException("Le XML doit contenir au moins une balise Contient ouNeContientPas.");
            String NomFichier = ValeurBalise (monXMLNode,"Fichier");
            NomFichier = JCAVariable.SubstituerVariables(NomFichier, Variables);
            Message = Message + Environment.NewLine + "Fichier à tester:" + NomFichier;
            if (!System.IO.File.Exists(NomFichier))
                throw new JCAssertionException("Le fichier " + NomFichier + "n'existe pas" );
            String Contenu = System.IO.File.ReadAllText(NomFichier );

            // traiter les valeurs multiples
            
            Boolean Resultat = true;
            String Contient = "";
            Boolean ResultatPartiel = true;
             
            foreach (XmlElement monFragmentXML in monXMLNode.SelectNodes("Contient"))
                {
                    
                    Contient = monFragmentXML.InnerText;
                    Contient = JCAVariable.SubstituerVariables(Contient, Variables);
                    Message= Message + Environment.NewLine + 
                        "Vérifier si le fichier contient:" +
                        Contient;
                    ResultatPartiel = Contenu.Contains(Contient);
                    if(!ResultatPartiel) 
                        {
                            Resultat = false;
                            MessageEchec = MessageEchec +
                                Environment.NewLine +
                                "Le texte '" + Contient + 
                                "' n'a pas été trouvé et il devrait être dans le fichier";
                        } //if(!ResultatPartiel) 
                } // foreach
                

                    String NeContientPas = "";


                foreach (XmlElement  monFragmentXMLCP in monXMLNode.SelectNodes(
                    "NeContientPas"))
                {
                    
                    NeContientPas = monFragmentXMLCP.InnerText;
                    NeContientPas = JCAVariable.SubstituerVariables(
                        NeContientPas, Variables);
                    Message= Message + Environment.NewLine + 
                        "Vérifier si le fichier ne contient pas:" +
                        NeContientPas;
                    ResultatPartiel = ! Contenu.Contains(NeContientPas);
                    if(!ResultatPartiel) 
                        {
                            Resultat = false;
                            MessageEchec = MessageEchec +
                                Environment.NewLine +
                                "Le texte '" + NeContientPas + 
                                "' a été trouvé et il me devrait pas être dans le fichier";
                        } // if(!ResultatPartiel)
                } // foreach 
                


            if (Resultat)
                Message = Message + Environment.NewLine +
                    "L'assertion est vraie";
            else
                {
                Message = Message + Environment.NewLine +
                    "L'assertion est fausse";
                // insérer le message d'échec
                if (ValeurBalise (monXMLNode,"MessageEchec") != "")
                    {
                        MessageEchec = ValeurBalise(monXMLNode, "MessageEchec") +
                            Environment.NewLine + MessageEchec;
                        MessageEchec = JCAVariable.SubstituerVariables(
                            MessageEchec , Variables);
 
                    }
                else
                {
                    MessageEchec = "Assertion fausse du fichier " + NomFichier  +
                        Environment.NewLine + MessageEchec;
                    MessageEchec = JCAVariable.SubstituerVariables(
                        MessageEchec, Variables);

                }

                }
            return Resultat ;
        }

        public static bool JCAExecuteProgramme(XmlNode monXMLNode, 
            ref string Message, ref  Dictionary<String, String> Variables,
            ref string MessageEchec)
            
        {
            JCAConsole maConsole = new JCAConsole();
            String Sortie = "";
            MessageEchec = "";
            Message = Message + Environment.NewLine + "Assertion ExecuteProgramme";
            if (monXMLNode == null) 
                throw new JCAssertionException("Le XML est vide.");
            
            // Valider le fichier de modèle

            ValideBalise(monXMLNode, "Programme");
            String Programme = ValeurBalise(monXMLNode, "Programme");
            Programme = JCAVariable.SubstituerVariables(Programme , Variables);
            
            String Arguments = ValeurBalise(monXMLNode, "Arguments");
            Arguments = JCAVariable.SubstituerVariables(Arguments , Variables);

            Boolean Resultat = (maConsole.ExecuteProgramme(Programme, 
                Arguments, ref Sortie) == 0);

            Message = Message + Environment.NewLine +
                "Résultat de l'exécution de " + Programme +
                Environment.NewLine + Sortie +
                Environment.NewLine + "Fin des résultats de " + Programme ;
            if (Resultat)
                Message = Message + Environment.NewLine +
                    "L'assertion est vraie";
            else
                {
                // echec
                Message = Message +
                Environment.NewLine +
                "L'assertion est fausse";
                // messageechec
                if (ValeurBalise (monXMLNode, "MessageEchec") != "")
                        MessageEchec = ValeurBalise (monXMLNode, "MessageEchec")
                            + " (Code de retour : " + 
                            maConsole.DernierCodeDeRetour.ToString() +
                             " )";
                    else
                        MessageEchec = 
                            "Le programme " + Programme  + 
                            " a terminé avec le code de retour " +
                            maConsole.DernierCodeDeRetour.ToString();
                }
                

            return Resultat;
        }

        public static bool JCAMAJVariables(
            XmlNode monXMLNode,
            ref string Message, 
            ref  Dictionary<String, String> Variables,
            ref String MessageEchec)
        {
            Message = Message + Environment.NewLine + 
                "Assertion MAJVariables" + Environment.NewLine  ;
            if (monXMLNode == null) throw new JCAssertionException("Le XML est vide.");
            ValideBalise(monXMLNode, "Cle");
            string MaCle = ValeurBalise (monXMLNode,"Cle");
            MaCle = JCAVariable.SubstituerVariables(MaCle, Variables);
            Message = Message + "Clé:" + MaCle + Environment.NewLine  ;
            ValideBalise(monXMLNode, "Valeur");
            
            string MaValeur = ValeurBalise (monXMLNode,"Valeur");
            MaValeur = JCAVariable.SubstituerVariables(MaValeur, Variables);
            Message = Message + "Valeur:" + MaValeur + Environment.NewLine;
            
            JCAVariable VariableTemp = new JCAVariable() ;
            VariableTemp.Variables  = Variables;
            VariableTemp.MAJVariable (MaCle, MaValeur  );
            Variables = VariableTemp.Variables;
            if(VariableTemp.GetValeurVariable(
                JCAVariable.Constantes.JCA_FichierDeVariables) != null )
                {
                    VariableTemp.EcrireFichier (VariableTemp.GetValeurVariable(
                        JCAVariable.Constantes.JCA_FichierDeVariables));
                         
                }



            Message = Message + Environment.NewLine +
                    "Valeur de variable mise à jour.";
            MessageEchec = "";
            
            
            return true ;
        }

        
     
        public bool JCASQLExecute(XmlNode monXMLNode,
             ref String Message,
             ref Dictionary<String, String>  Variables,
             ref string   MessageEchec,
             ref JCASQLClient  monSQLClient)
                 {
                     Message = Message + Environment.NewLine +
                        "Assertion :  SQLExecute" + Environment.NewLine;
                     MessageEchec = "";
                     if (monXMLNode == null)
                         throw new JCAssertionException("Le XML est vide.");
                     ValideBalise(monXMLNode, "SQL");
                     String monSQL = "";        
            Int64 Rangees = 0;
            foreach (XmlElement  monFragmentXML in monXMLNode.SelectNodes(
                    "SQL"))
                {
                    monSQL = monFragmentXML.InnerText;
                    monSQL = JCAVariable.SubstituerVariables(
                    monSQL, Variables);
                    Message = Message + monSQL + Environment.NewLine ;
                    Rangees = monSQLClient.SQLExecute(monSQL);
                if (Rangees > 1)
                    Message = Message + Rangees.ToString() +
                        " rangées affectées." + Environment.NewLine;
                else 
                    Message = Message + Rangees.ToString() +
                        " rangée affectée." + Environment.NewLine;
                }
            
            
            return true ;
                 }

        public bool JCAChargeLOB(XmlNode monXMLNode,
            ref string Message,
            ref  Dictionary<String, String> Variables,
            ref string MessageEchec,
            ref JCASQLClient monSQLClient)
        {
            Message = Message + Environment.NewLine +
            "Assertion ChargeLOB" + Environment.NewLine;
            MessageEchec = "";
            if (monXMLNode == null)
                throw new JCAssertionException("Le XML est vide.");
            ValideBalise(monXMLNode, "SQL");
            ValideBalise(monXMLNode, "Fichier");
            
            String monSQL = ValeurBalise(monXMLNode, "SQL");
            monSQL = JCAVariable.SubstituerVariables(
                    monSQL, Variables);
            String monFichier = ValeurBalise(monXMLNode, "Fichier");
            monFichier = JCAVariable.SubstituerVariables(
                    monFichier, Variables);

            Message = Message + "SQL de spécification des rangées à charger "
                + monSQL + Environment.NewLine;
            Message = Message + "Fichier dont le contenu sera chargé : " +
                monFichier + Environment.NewLine;
            
            
            long Rangees =
                monSQLClient.SQLChargeLOB(monSQL, monFichier) ;

           if (Rangees > 1)
                    Message = Message + Rangees.ToString() +
                        " rangées affectées." + Environment.NewLine;
                else 
                    Message = Message + Rangees.ToString() +
                        " rangée affectée." + Environment.NewLine;
                
            
            return true;
        }

        public bool JCAExporteLOB(XmlNode monXMLNode,
            ref string Message,
            ref  Dictionary<String, String> Variables,
            ref string MessageEchec,
            ref JCASQLClient monSQLClient)
        {
            Message = Message + Environment.NewLine +
            "Assertion ExporteLOB" + Environment.NewLine;
            MessageEchec = "";
            if (monXMLNode == null)
                throw new JCAssertionException("Le XML est vide.");
            ValideBalise(monXMLNode, "SQL");
            ValideBalise(monXMLNode, "Chemin");

            String monSQL = ValeurBalise(monXMLNode, "SQL");
            monSQL = JCAVariable.SubstituerVariables(
                    monSQL, Variables);
            String monChemin = ValeurBalise(monXMLNode, "Chemin");
            monChemin = JCAVariable.SubstituerVariables(
                    monChemin, Variables);

            String monEncodageStr = ValeurBalise(monXMLNode, "Encodage");
            monEncodageStr = JCAVariable.SubstituerVariables(
                monEncodageStr, Variables).ToUpper() ;
            Encoding monEncodage = Encoding.UTF8;
            if (monEncodageStr.Contains("ASCII"))
                monEncodage = Encoding.ASCII;
            

            Message = Message + "SQL de spécification des rangées à exporter "
                + monSQL + Environment.NewLine;
            Message = Message + "Chemin des fichiers créés : " +
                monChemin + Environment.NewLine;


            long Rangees =
                monSQLClient.ExporteLOB(monSQL,
                monChemin, monEncodage);
            Message = Message + monSQLClient.DernierResultat +
                Environment.NewLine; 
            if (Rangees > 1)
                Message = Message + Rangees.ToString() +
                    " rangées exportées." + Environment.NewLine;
            else
                Message = Message + Rangees.ToString() +
                    " rangée exportée." + Environment.NewLine;


            return true;
        }

        


    }
}
