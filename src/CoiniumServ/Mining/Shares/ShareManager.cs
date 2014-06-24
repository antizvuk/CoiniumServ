﻿/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using Coinium.Coin.Daemon;
using Coinium.Mining.Jobs;
using Coinium.Server.Stratum;

namespace Coinium.Mining.Shares
{
    public class ShareManager : IShareManager
    {
        private readonly IJobManager _jobManager;
        private readonly IDaemonClient _daemonClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareManager" /> class.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="jobManager">The job manager.</param>
        /// <param name="daemonClient"></param>
        public ShareManager(IJobManager jobManager, IDaemonClient daemonClient)
        {
            _jobManager = jobManager;
            _daemonClient = daemonClient;
        }

        /// <summary>
        /// Processes the share.
        /// </summary>
        /// <param name="miner">The miner.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="extraNonce2">The extra nonce2.</param>
        /// <param name="nTimeString">The n time string.</param>
        /// <param name="nonceString">The nonce string.</param>
        /// <returns></returns>
        public IShare ProcessShare(StratumMiner miner, string jobId, string extraNonce2, string nTimeString, string nonceString)
        {
            // check if the job exists
            var id = Convert.ToUInt64(jobId, 16);
            var job = _jobManager.GetJob(id);

            // create the share
            var share = new Share(id, job, _jobManager.ExtraNonce.Current, extraNonce2, nTimeString, nonceString);

            //var target = new BigInteger(job.EncodedDifficulty, 16);
            //if (target.Subtract(headerValue).IntValue > 0) // Check if share is a block candidate (matched network difficulty)
            //{
            //    var blockHex = Serializers.SerializeBlock(job, header, coinbase).ToHexString();

            //    // we should be using another scrypt hash here? - https://github.com/zone117x/node-stratum-pool/blob/eb4b62e9c4de8a8cde83c2b3756ca1a45f02b957/lib/jobManager.js#L232

            //    // return SubmitBlock(blockHex);
            //}
            //else // invalid share.
            //{
            //    // TODO: implement me
            //}

            return share;
        }

        private bool SubmitBlock(string blockHex)
        {
            var response = _daemonClient.SubmitBlock(blockHex).ToLower();

            if (response == "accepted")
                return true;
            else if (response == "rejected")
                return false;

            return false;
        }
    }
}