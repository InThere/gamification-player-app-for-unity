using System;
using GamificationPlayer.DTO.ExternalEvents;
using NUnit.Framework;
using UnityEngine;

namespace GamificationPlayer.Tests
{
    public class PageViewDTOTest
    {
        [Test]
        public void TestToJSON()
        {
            var obj = new PageViewDTO();

            obj.data.attributes.organisation_id = Guid.NewGuid().ToString();
            obj.data.attributes.user_id = Guid.NewGuid().ToString();

            obj.data.type = "pageView";

            var json = obj.ToJson();

            Assert.That(json.Contains(obj.data.attributes.organisation_id));
            Assert.That(json.Contains(obj.data.attributes.user_id));

            Assert.That(json.Contains(obj.data.type));
        }

        [Test]
        public void TestFromJSON()
        {
            var obj = new PageViewDTO();

            obj.data.attributes.organisation_id = Guid.NewGuid().ToString();
            obj.data.attributes.user_id = Guid.NewGuid().ToString();

            obj.data.type = "pageView";

            var json = obj.ToJson();
            var newObj = json.FromJson<PageViewDTO>();

            Assert.AreEqual(newObj.data.attributes.organisation_id, obj.data.attributes.organisation_id);
            Assert.AreEqual(newObj.data.attributes.user_id, obj.data.attributes.user_id);

            Assert.AreEqual(newObj.data.type, obj.data.type);
        }
    }
}