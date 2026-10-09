using System.Collections;
using System.IO;
using System.Linq;
using FXR.Memory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class MemoryRoomIntegrationTests
{
    [UnityTest]
    public IEnumerator GeneratedSceneRunsBothRoundsAndResetRestoresPhysics()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainScene");
        yield return null;
        var roomObject = GameObject.Find("MemoryRoom");
        if (roomObject == null) throw new System.Exception("Room missing. Active scene roots: " + string.Join(",", UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Select(x => x.name)));
        Assert.That(roomObject != null, Is.True, "MainScene must contain MemoryRoom");
        var room = roomObject.GetComponent<MemoryRoomController>();
        if (room == null) throw new System.Exception("Room controller missing from MemoryRoom.");
        if (room.cards == null) throw new System.Exception("Card array missing from serialized room controller.");
        Assert.That(room != null, Is.True);
        Assert.That(room.cards.Length, Is.EqualTo(12));
        Assert.That(room.officeAudio.clip, Is.Not.Null);
        Assert.That(room.natureAudio.clip, Is.Not.Null);
        var homes = room.cards.Select(card => card.transform.localPosition).ToArray();
        room.actionButton.onClick.Invoke();
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Running));
        Assert.That(room.officeProps.activeSelf, Is.True);
        Assert.That(room.natureProps.activeSelf, Is.False);
        var exportFolder = Path.Combine(Application.persistentDataPath, "MemoryResults");
        var existingExports = Directory.Exists(exportFolder) ? Directory.GetFiles(exportFolder) : new string[0];
        for (int round = 0; round < 2; round++)
        {
            for (int pair = 0; pair < 6; pair++)
            {
                var indices = Enumerable.Range(0, 12).Where(i => room.Session.PairIds[i] == pair).ToArray();
                room.cards[indices[0]].Select();
                room.cards[indices[1]].Select();
                yield return new WaitForSeconds(.55f);
            }
            Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Rating));
            Assert.That(room.ratingPanel.activeSelf, Is.True);
            Assert.That(room.cards.All(c => !c.GetComponent<Rigidbody>().isKinematic), Is.True);
            room.RateEffort(round == 0 ? 4 : 2);
            if (round == 0)
            {
                room.actionButton.onClick.Invoke();
                Assert.That(room.natureProps.activeSelf, Is.True);
                Assert.That(room.officeProps.activeSelf, Is.False);
            }
        }
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Completed));
        var newExports = Directory.GetFiles(exportFolder).Except(existingExports).ToArray();
        Assert.That(newExports.Length, Is.EqualTo(1));
        Assert.That(File.ReadAllLines(newExports[0]).Length, Is.EqualTo(3));
        File.Delete(newExports[0]);
        room.ResetRoom();
        Assert.That(room.Session.Results, Is.Empty);
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Idle));
        for (int i = 0; i < room.cards.Length; i++)
        {
            Assert.That(room.cards[i].transform.localPosition, Is.EqualTo(homes[i]));
            Assert.That(room.cards[i].front.activeSelf, Is.False);
            Assert.That(room.cards[i].GetComponent<Rigidbody>().isKinematic, Is.True);
        }
    }
}
