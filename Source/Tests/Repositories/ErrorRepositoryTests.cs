﻿#region Copyright 2014 Exceptionless

// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
//     http://www.gnu.org/licenses/agpl-3.0.html

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exceptionless.Core;
using Exceptionless.Models;
using Exceptionless.Tests.Utility;
using MongoDB.Bson;
using Xunit;

namespace Exceptionless.Tests.Repositories {
    public class ErrorRepositoryTests : MongoRepositoryTestBaseWithIdentity<Error, IErrorRepository> {
        private readonly IErrorStackRepository _errorStackRepository = IoC.GetInstance<IErrorStackRepository>();

        public ErrorRepositoryTests() : base(IoC.GetInstance<IErrorRepository>(), true) {}

        [Fact]
        public void GetPreviousErrorOccurrenceIdTest() {
            Debug.WriteLine("Actual order:");
            foreach (var t in _ids)
                Debug.WriteLine("{0}: {1}", t.Item1, t.Item2.LocalDateTime.ToLongTimeString());

            Debug.WriteLine("");
            Debug.WriteLine("Sorted order:");
            List<Tuple<string, DateTimeOffset>> sortedIds = _ids.OrderBy(t => t.Item2.Ticks).ThenBy(t => t.Item1).ToList();
            foreach (var t in sortedIds)
                Debug.WriteLine("{0}: {1}", t.Item1, t.Item2.LocalDateTime.ToLongTimeString());

            Debug.WriteLine("");
            Debug.WriteLine("Tests:");
            Assert.Equal(_ids.Count, Repository.Count());
            for (int i = 0; i < sortedIds.Count; i++) {
                Debug.WriteLine("Current - {0}: {1}", sortedIds[i].Item1, sortedIds[i].Item2.LocalDateTime.ToLongTimeString());
                if (i == 0)
                    Assert.Null(Repository.GetPreviousErrorOccurrenceId(sortedIds[i].Item1));
                else
                    Assert.Equal(sortedIds[i - 1].Item1, Repository.GetPreviousErrorOccurrenceId(sortedIds[i].Item1));
            }
        }

        [Fact]
        public void GetNextErrorOccurrenceIdTest() {
            Debug.WriteLine("Actual order:");
            foreach (var t in _ids)
                Debug.WriteLine("{0}: {1}", t.Item1, t.Item2.LocalDateTime.ToLongTimeString());

            Debug.WriteLine("");
            Debug.WriteLine("Sorted order:");
            List<Tuple<string, DateTimeOffset>> sortedIds = _ids.OrderBy(t => t.Item2.Ticks).ThenBy(t => t.Item1).ToList();
            foreach (var t in sortedIds)
                Debug.WriteLine("{0}: {1}", t.Item1, t.Item2.LocalDateTime.ToLongTimeString());

            Debug.WriteLine("");
            Debug.WriteLine("Tests:");
            Assert.Equal(_ids.Count, Repository.Count());
            for (int i = 0; i < sortedIds.Count; i++) {
                Debug.WriteLine("Current - {0}: {1}", sortedIds[i].Item1, sortedIds[i].Item2.LocalDateTime.ToLongTimeString());
                if (i == sortedIds.Count - 1)
                    Assert.Null(Repository.GetNextErrorOccurrenceId(sortedIds[i].Item1));
                else
                    Assert.Equal(sortedIds[i + 1].Item1, Repository.GetNextErrorOccurrenceId(sortedIds[i].Item1));
            }
        }

        private readonly List<Tuple<string, DateTimeOffset>> _ids = new List<Tuple<string, DateTimeOffset>>();

        protected override void CreateData() {
            var occurrenceDateStart = new DateTimeOffset(DateTime.Now.AddMinutes(-30));
            var occurrenceDateMid = new DateTimeOffset(DateTime.Now);
            var occurrenceDateEnd = new DateTimeOffset(DateTime.Now.AddMinutes(30));

            _errorStackRepository.Add(ErrorStackData.GenerateErrorStack(id: TestConstants.ErrorStackId, organizationId: TestConstants.OrganizationId, projectId: TestConstants.ProjectId));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateStart));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateEnd));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), new DateTimeOffset(DateTime.Now.AddMinutes(-10))));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), new DateTimeOffset(DateTime.Now.AddMinutes(-20))));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateMid));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateMid));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateMid));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), new DateTimeOffset(DateTime.Now.AddMinutes(20))));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), new DateTimeOffset(DateTime.Now.AddMinutes(10))));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), new DateTimeOffset(DateTime.Now.AddSeconds(1))));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateEnd));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));

            _ids.Add(Tuple.Create(ObjectId.GenerateNewId().ToString(), occurrenceDateStart));
            Repository.Add(ErrorData.GenerateError(id: _ids.Last().Item1, projectId: TestConstants.ProjectId, organizationId: TestConstants.OrganizationId, errorStackId: TestConstants.ErrorStackId, occurrenceDate: _ids.Last().Item2, nestingLevel: 5, minimiumNestingLevel: 1));
        }

        protected override void RemoveData() {
            base.RemoveData();
            _errorStackRepository.DeleteAll();
        }
    }
}