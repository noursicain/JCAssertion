﻿// License
//
//
//  JCAssertion : Un ensemble d’outils
//              pour configurer et vérifier les environnements 
//              de tests sous windows.
//
//  Copyright 2016 Jean-Claude Parent 
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
using JCASQLODPCore;

namespace JCAssertionCore
{
    /// <summary>
    /// JCAODPSQLClient : Classe définissant les propriétés et méthodes
    /// de plus haut niveau pour le pont xml
    /// pour accéder une base de données oracle avec le oracle data provider.
    /// </summary>
    public class JCAODPSQLClient
    {

        private JCASQLODPClient monSQLClient = new JCASQLODPClient();
        public enum Action {Aucune, Ouvrir, Fermer };

        public Boolean ConnectionOuverte()
        {
            return monSQLClient.SiConnectionOuverte(); 
        }


        /// <summary>
        /// InitConnection : Définit les param
        /// etres de la
        /// connection et permet d'ouvrir
        /// retourne la chaîne de conntextion ou de ferner celle-ci
        /// </summary>
        public void InitConnection(String User,
            String Password,
            String Serveur = null,
            Action monAction = Action.Aucune  )
        {
            monSQLClient.User = User;
            monSQLClient.Password = Password;
            monSQLClient.Serveur = Serveur;
            if (monAction == Action.Ouvrir)
                monSQLClient.OuvrirConnection();
            if (monAction == Action.Fermer)
                monSQLClient.FermerConnection(); 
        }


        public Boolean SQLAssert(String SQL,
            String ResultatAttendu)
        {
            Boolean Resultat = false;
            Boolean Fermer = (!monSQLClient.SiConnectionOuverte());
            if (!monSQLClient.SiConnectionOuverte())
                monSQLClient.OuvrirConnection();
            Resultat = monSQLClient.AssertSQLString(SQL, ResultatAttendu); 
            if (Fermer)
                monSQLClient.FermerConnection();

            return Resultat; 
        }

        public Boolean SQLAssert(String SQL,
            Double  ResultatAttendu)
        {
            return false;
        }



    }
}
