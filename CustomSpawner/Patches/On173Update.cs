using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static HarmonyLib.AccessTools;
using NorthwoodLib.Pools;
using PlayableScps;

namespace CustomSpawner.Patches
{
    [HarmonyPatch(typeof(Scp173), nameof(Scp173.OnUpdate))]
    internal static class Scp173Update
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp173), nameof(Scp173.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(DummiesManager), nameof(DummiesManager.IsDummy), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Brtrue_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}