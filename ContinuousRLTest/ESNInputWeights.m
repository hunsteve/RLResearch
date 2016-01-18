function I = ESNInputWeights(inputcount)
I = (rand(hiddencount,inputcount + 1) - 0.5) * 2;
