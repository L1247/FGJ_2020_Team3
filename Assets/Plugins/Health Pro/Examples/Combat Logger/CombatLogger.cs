using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;

namespace Lowscope.HealthPro.Examples
{
    [AddComponentMenu("")]
    public class CombatLogger : MonoBehaviour, IDamageable, IHealable, IDamageCallback, IKillable, IInvulnerable
    {
        [SerializeField]
        private Text text = null;

        private StringBuilder stringBuilder = new StringBuilder();

        private List<string> messages = new List<string>();

        public void BecameInvulnerable(bool isInvulnerable)
        {
            messages.Add(string.Format("You have become {0}", isInvulnerable ? "invulnerable" : "vulnerable"));
            DrawLog();
        }

        public void OnDamaged(HealthInfo healthInfo)
        {
            messages.Add(string.Format("{0} has damaged you for {1} points.", (healthInfo.CauseIdentifier != 0)? healthInfo.Cause.name : "", healthInfo.Amount));
            DrawLog();
        }

        public void OnDamageDone(HealthInfo info)
        {
            Health target = info.Owner;

            if (info.CurrentHealth > 0)
            {
                if (info.Amount > 0)
                {
                    messages.Add(string.Format("You have damaged {0} for {1} points.", target.name, info.Amount));
                }
                else
                {
                    messages.Add(string.Format("You have healed {0} for {1} points.", target.name, info.Amount));
                }
            }
            else
            {
                messages.Add(string.Format("You have killed {0}", target.name));
            }

            DrawLog();
        }

        public void OnDeath(HealthInfo healthInfo)
        {
            messages.Add("You have died.");
            DrawLog();
        }

        public void OnHealed(HealthInfo healthInfo)
        {
            messages.Add(string.Format("You have been healed for {0} points", healthInfo.Amount));
            DrawLog();
        }

        private void DrawLog()
        {
            // Clear all characters from the string builder
            stringBuilder.Remove(0, stringBuilder.Length);

            while (messages.Count > 5)
            {
                messages.RemoveAt(0);
            }

            for (int i = 0; i < messages.Count; i++)
            {
                stringBuilder.AppendLine(messages[i]);
            }

            text.text = stringBuilder.ToString();
        }
    }

}
