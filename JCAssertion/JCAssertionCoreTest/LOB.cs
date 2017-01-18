﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using JCAssertionCore;
  

namespace JCAssertionCoreTest
{
    [TestClass]
    public class LOB
    {

        XmlDocument monCas;
        JCACore monCore;
        String Chemin = JCAssertionCore.JCACore.RepertoireAssembly() +
                "Ressources\\";
        public void InitBD()
            {
                monCas.InnerXml = "<Assertion>" +
                   "<Type>SQLExecute</Type>" +
                   "<SQL>delete from JCATest where idtest='JCACT.LOB_1'</SQL>" +
                   "<SQL>INSERT INTO JCATest(IDTEST) VALUES ('JCACT.LOB_1')</SQL>" +
                   "</Assertion>";

                if (!monCore.ExecuteCas(monCas))
                    throw new Exception(
                        "Erreur d'initialisation " +
            monCore.Message + " " +
            monCore.MessageEchec );
            
            }
        

        [TestInitialize]
        public void InitTest()
        {
            // Définir connection
            monCas = new XmlDocument();
            monCas.InnerXml = "<Assertion>" +
               "<Type>ConnectionOracle</Type>" +
               "<User>JCA</User>" +
               "<Password>JCA</Password>" +
               "</Assertion>";

            monCore = new JCACore();
            monCore.ExecuteCas(monCas);
            monCore.Variables.MAJVariable(
                "Fichier", Chemin + "FichierDeCasOK.xml");
            


        }

        
 

        [TestCleanup]
        public void CleanTest()
        {
            
            // La connection s'étant fermée à chaque assertionn
            // ps besoin de ;a fermer
            
        }
    

        [TestMethod]
        public void LOBOKUT1()
        {
            InitBD();
            monCore.Variables.MAJVariable("Where",
                " idtest='JCACT.LOB_1'");  
            // Lancer un test où les assertions passent
            monCas.InnerXml = "<Assertion>" +
               "<Type>ChargeLOB</Type>" +
               "<Fichier>{{Fichier}}</Fichier>" +
               "<SQL>select idtest, typeblob AS BLOB from JCATest where {{Where}}</SQL>" +
               "</Assertion>";

            if (!monCore.ExecuteCas(monCas))
                throw new Exception("Échec de l'assertion "+
            monCore.Message + 
            monCore.MessageEchec  );

            Assert.IsTrue(monCore.Message.Contains("SQL de spécification des rangées à charger select idtest, typeblob AS BLOB from JCATest where  idtest='JCACT.LOB_1'"),
                "Attendu : SQL de spécification des rangées à charger select idtest, typeblob AS BLOB from JCATest where  idtest='JCACT.LOB_1'");
            Assert.IsTrue(monCore.Message.Contains("Fichier dont le contenu sera chargé :"),
                "Attendu : Fichier dont le contenu sera chargé :");
            Assert.IsTrue(monCore.Message.Contains("1 rangée affectée"),
                "Attendu : 1 rangée affectée"); 
            
            Assert.Fail("Pas encore implémenté "  +
            monCore.Message );
        }
    }
}
