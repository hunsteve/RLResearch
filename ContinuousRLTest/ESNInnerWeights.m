function M = ESNInnerWeights(hiddencount, ratio)
M = zeros(hiddencount,hiddencount);

ind = ceil(bsxfun(@times,size(M),rand(round(hiddencount^2*ratio),2)));

ind = sub2ind(size(M),ind(:,1),ind(:,2));

plus_minus_ones = sign(randn(size(ind)));

M(ind) = plus_minus_ones;

M = M * 0.95 * 1 / max(abs(eig(M)));