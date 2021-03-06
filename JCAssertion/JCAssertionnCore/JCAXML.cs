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

namespace JCAssertionCore
{
    public class JCAXML
    {

        
        
        
        /// <summary>
        /// Fait une assertion logique
        /// sur le nombre de noeuds 
        /// retournés par une expression XPath
        /// sur un fichier xml
        /// </summary>
        /// <param name="FichierXML">Chemin et nom du fichier XML</param>
        /// <param name="XPath">Expression XPath</param>
        /// <param name="Operateur">Operateur de comparaison avec le résultat attendu</param>
        /// <param name="ResultatAttendu">Resultat à comparer</param>
        /// <param name="ResultatReel">Retourne le nombre de noeuds retournés par l'expression</param>
        /// <returns>La comparaison est vraie ou fausse</returns>
        public bool AssertXPath(String FichierXML,
            String XPath,
            String Operateur,
            Int64 ResultatAttendu,
            ref Int64 ResultatReel,
            String Recherche = null,
            Boolean SensibleCase = true )
            {
                if (Recherche == "")
                    Recherche = null;

                if (Operateur == null)
                    Operateur = "PG";
                if (Operateur == "")
                    Operateur = "PG";
                Operateur = Operateur.ToUpper();
                

                XmlDocument monXML = new XmlDocument();
                monXML.Load(FichierXML);
                
                XmlNodeList maListe = monXML.SelectNodes(XPath );
                if (Recherche == null)
                    ResultatReel = maListe.Count;
                else
                    ResultatReel = XMLRecherche(maListe,
                        Recherche , SensibleCase );
                switch  (Operateur) 
                { 
                    case "=":
                        return (ResultatReel == ResultatAttendu);
                    case "PG":
                        return (ResultatReel > ResultatAttendu);
                    case "PP":
                        return (ResultatReel < ResultatAttendu);
                    case ">":
                        return (ResultatReel > ResultatAttendu);
                    case "<":
                        return (ResultatReel < ResultatAttendu);
                    case ">=":
                        return (ResultatReel >= ResultatAttendu);
                    case "<=":
                        return (ResultatReel <= ResultatAttendu);
                    case "PG=":
                        return (ResultatReel >= ResultatAttendu);
                    case "PP=":
                        return (ResultatReel <= ResultatAttendu);
                    case "!=":
                        return (ResultatAttendu != ResultatReel);
                    case "<>":
                        return (ResultatAttendu != ResultatReel);
                    default :
                        throw new JCAssertionException(
                            "Pour une assertion XPath l'opérateur '" +
                    Operateur + "' n'est pas un opérateur valide.");


                    }
                
            }


        private Int64 XMLRecherche(
            XmlNodeList ListeNoeud,
            String ChaineRecherche,
            Boolean SensibleCase = true )
            {
                Int64 Resultat = 0;
                foreach (XmlElement  monNoeud in ListeNoeud)
                    {
                        
                        if ((SensibleCase) && (
                            monNoeud.InnerText.Contains(ChaineRecherche)))
                            Resultat = Resultat + 1;
                        else
                            if ((!SensibleCase) && (
                            monNoeud.InnerText.ToUpper().Contains(
                            ChaineRecherche.ToUpper())))
                                Resultat = Resultat + 1;

                    }
                
                return Resultat;
            }
    }
}
