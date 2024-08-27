# Cleverbit-Tech-Test

## Setup

```bash
kubectl create ns flux-system

cd cluster/flux-system

kustomize build . | kubectl apply -f -

(kubectl apply -f gotk-components.yaml
kubectl apply -f gotk-sync.yaml)

kubectl get kustomizations.kustomize.toolkit.fluxcd.io -A

kubectl create ns authority
kubectl create ns servicex
kubectl create ns servicey
kubectl create ns istiosystem

istioctl install --set profile=default -y
```
![image](https://github.com/user-attachments/assets/c84ddb39-5d58-4235-a112-bfd2f84217a5)
![image](https://github.com/user-attachments/assets/fa60ab23-0399-45be-9d3b-7884db2e13a2)
