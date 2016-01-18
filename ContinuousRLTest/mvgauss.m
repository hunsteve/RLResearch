function prob = mvgauss(x, mean, covariance)
prob = mvnpdf(x, mean, covariance);