﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAssertionCore;


namespace JCAssertionCoreTest
{
    [TestClass]
    public class JCAUtilitaireUnitTest1
    {
        [TestMethod]
        public void EventLogErreur()
        {
            String IDunique = Guid.NewGuid().ToString ();
            JCAUtilitaires U = new JCAUtilitaires();

            Assert.IsFalse(JCAUtilitaires.EVSourceExiste("Sourceindexistante"),
                "Sourceindexistante ne devrait pas exister commesource de event log"); 
            Assert.IsTrue (JCAUtilitaires.EVSourceExiste(),
                "JCAssertion devrait exister comme source de event log");
            // RechercheJournalEve
            Assert.IsFalse(
                U.RechercheJournalEve("JCAssertion", IDunique ).Contains(IDunique ) );
 
            
            JCAssertionCore.JCAUtilitaires.EventLogErreur(" ID unique = " + IDunique );
            
            String Journal = U.RechercheJournalEve("JCAssertion", 
                IDunique);

            Assert.IsTrue(
                Journal.Contains(IDunique),
                "Identifiant unique non trouvé dans " +
                Journal );
 

        }
    }
}
