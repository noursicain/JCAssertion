﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAssertionCore;



namespace JCAssertionCoreTest
{
    [TestClass]
    public class CoreUnitTest
    {
        [TestMethod]
        public void CoreLoadOK()
        {
            // test limité sans fichier de valeurs

            Core monJCACore = new Core();
            Assert.AreEqual(1, monJCACore ,"Le fichier chargé devrait contenir 1 cas de test, réel = " + monJCACore.NombreCas.ToString()  );
            monJCACore.Load(Core.RepertoireAssembly() + "Ressources\\FichierDeCasOK.xml");
            Assert.IsTrue(monJCACore.FichierDeCas.Contains("Ressources\\FichierDeCasOK.xml"));
            Assert.IsNull(monJCACore.FichierValeur);
            Assert.IsNotNull(monJCACore.FichierJournal);
 
            // Test plus étendu avec fichier de valeur
            Assert.Fail("Oas encore implémenté.");

            
        }
    }
}
