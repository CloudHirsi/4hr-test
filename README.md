# Cleverbit-Tech-Test

## Setup

```bash
kubectl create ns flux-system
cd apps/cluster
kustomize build . | kubectl apply -f -
```
