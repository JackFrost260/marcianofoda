
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections.Generic;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace Demos
    {

        namespace TheLastParticle
        {

            // =================================	
            // Classes.
            // =================================

            //[ExecuteInEditMode]
            [System.Serializable]

            public class RandomSequence
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                int currentIndex;
                int[] randomSequence;

                // =================================	
                // Functions.
                // =================================

                // NON-REPEATING random sequence from range [0, rangeMax] repeated 'loopCount' number of times.
                // Total size of returned array = rangeMax * length.

                public RandomSequence(int rangeMax, int loopCount)
                {
                    int lastRandomNumber = -1;
                    randomSequence = new int[rangeMax * loopCount];

                    int count = 0;

                    for (int i = 0; i < loopCount; i++)
                    {
                        List<int> intList = new List<int>();

                        for (int j = 0; j < rangeMax; j++)
                        {
                            intList.Add(j);
                        }

                        for (int j = 0; j < rangeMax; j++)
                        {
                            int index = Random.Range(0, intList.Count);

                            int randomNumber = intList[index];

                            // When the pattern repeats, the last random number
                            // may still be selected again since the sequence was
                            // refreshed. 

                            // Example: [0, 2, 1] and then [1, 2, 0].

                            // In this case, although each pattern has no repeat,
                            // the total array ends up being [0, 2, 1, 1, 2, 0].

                            // This can only happen if both the loopCount and rangeMax
                            // values are at least 2 and if on the first pick of the 
                            // iteration following the previous (i > 0 && j == 0). 
                            // I only check if j == 0 because of the number of calculations
                            // avoided by the if-statement is best this way.

                            // If rangeMax is 1, then of course repeating is unavoidable
                            // because there's only one number to pick from, so ignore if
                            // range max is not > 1. Else, just cycle to the next number
                            // to make sure it won't be the same as the last number selected.

                            // If there's only one number, the sequence will always be [0, 0, 0, 0...].
                            // If there's only two numbers, the sequence will always be [0, 1, 0, 1...].

                            if (j == 0)
                            {
                                if (randomNumber == lastRandomNumber && rangeMax > 1)
                                {
                                    index = (index + 1) % intList.Count;
                                    randomNumber = intList[index];
                                }
                            }

                            randomSequence[count] = randomNumber;
                            lastRandomNumber = randomNumber;
                            
                            intList.RemoveAt(index); count++;
                        }
                    }
                }

                public int get()
                {
                    int value = randomSequence[currentIndex];
                    currentIndex = (currentIndex + 1) % randomSequence.Length;

                    return value;
                }

                // =================================	
                // End functions.
                // =================================

            }

            // =================================	
            // End namespace.
            // =================================

        }

    }

}

// =================================	
// --END-- //
// =================================
