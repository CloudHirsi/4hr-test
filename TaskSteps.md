# Task 1

Using the Dockerfiles located in the `/src/authority` and `/src/s2s` directories, create two images. The names of the images will be `authority` and `s2s`. You will assign the tags for these images.

During development, you can use your local container registry or the local cache system of the Kubernetes distribution you are using. To complete the task, upload the final images to your personal public repository and ensure they are accessible.

# Task 2

The repository provided to you is a Flux repository. The entry point for Flux is under the `/cluster/flux-system` folder. Extract the ZIP file, and then upload it to your personal repository along with your results.

**Step 2.1:**
Initiate the flux bootstrap process. Do not use the flux CLI during this process. Set up Flux and complete your bootstrap process. At this point, bootstrap `/cluster/flux-system/` using `kubectl apply`. All actions and changes you make, other than Flux, must be applied to the Kubernetes environment via Flux.

**Step 2.2:**
This repository will already set up the Istio service mesh. You are required to lock all Istio-related HelmReleases to version 1.20.0.

**Step 2.3:**
An Istio Gateway will be installed to control the ingress network. You are to install the Istio GW with the HelmRelease name `istio-ingress` in a namespace of the same name. The HelmRelease should depend on `istio-base` and `istiod` releases. Write the relevant release in the `/tools/istio` folder in `istio-gw.yaml` and ensure it becomes part of the Istio customization.

**Step 2.4:**
With the development you will make in `/tools/mtls/mtls.yaml`, enforce mTLS communication for all services within the mesh.

**Note:**
A dummy certificate is provided to you in `/tools/tls/tls.yml` for the domain `cb-interview.com`. You may trust this dummy certificate on your local machine to simplify the subsequent process for yourself.

#Task 3:

At this stage, you will install the applications for which you have prepared images. The details of the installation will be described in the relevant step below. Apart from what is specified in the task steps, feel free to use and do not hesitate to apply best practices.

**General Requirements for All Applications:**

1. All applications you install must be included in the mesh.

2. Ingress traffic will be managed by the Istio GW you have installed. TLS termination will be done using the `cb-tls` secret located under the `istio-ingress` namespace when the GW is used.

3. GW definitions should accept HTTP traffic but must perform a 301 HTTPS redirect.

4. Your `istio-ingress` load balancer service should acquire an external IP of 127.0.0.1 on your host. Depending on your Kubernetes distribution, you might need to use `minikube -> tunnel` or `kind -> metalLB`.

5. Create a ConfigMap with Key: `Cleverbit__Enabled`, Value: `true`.

6. Mount this ConfigMap in your pods under `/app/kpf/enable-config`. The mount should be in the key-per-file format.

7. Incoming traffic to the container should be through port 8080.

8. (Preferably) After TLS termination, the traffic to your container should remain on the HTTP protocol until it reaches port 8080.

**Step 3.1:**

Install the authority application under `/apps/authority/`. Requirements for the application:

1. Write a Deployment for the authority application. Your main container will be the `authority` image.

2. Expose this service using the gateway.

3. The service should use `authority.cb-interview.com` as the host name in the gateway.

4. Your application should use the `authority` namespace.

**Step 3.2:**

Install the servicex application under `/apps/servicex`. Requirements for the application:

1. Write a Deployment for the servicex application. Your main container will be the `s2s` image.

2. Expose this service using the gateway.

3. The service should use `servicex.cb-interview.com` as the host name in the gateway.

4. Create a secret for this service with key: `Cleverbit__Url` and value: `http://servicey.servicey.svc.cluster.local`. Adjust this address according to your cluster domain.

5. This secret will be used for s2s communication. Mount the secret under `/app/kpf/secret` path.

6. The service should use `servicex.cb-interview.com` as the host name in the gateway.

7. Your application should use the `servicex` namespace.

8. Your application should only accept requests carrying bearer JWTs with a valid issuer `testing@secure.istio.io`. Use Istio for this. Use `https://raw.githubusercontent.com/istio/istio/release-1.20/security/tools/jwt/samples/jwks.json` as your JWK keystore for JWT.

9. Forward the payload found in your bearer token to the `X-JWT-PAYLOAD` header.

10. Tokens should pass through Envoy to the service.

**Step 3.3:**

Install the servicey application under `/apps/servicey`. Requirements for the application:

1. Write a Deployment for the servicey application. Your main container will be the `s2s` image.

2. Do not expose this service using the gateway.

3. Your application should use the `servicey` namespace.

4. JWT requirements valid for servicex are also valid for this service.

---

**Task 4 (Validation):**

If you have completed all tasks, executing the following commands in order should result in a response of `Success!`. If you receive a different result, please review the steps you've implemented above.
1. Retrieve a JWT from the relevant service by running the command:
   ```
   $token = curl https://authority.cb-interview.com/token --resolve authority.cb-interview.com:443:127.0.0.1
   ```
2. Receive a response of `Success!` by running the command:
   ```
   curl https://servicex.cb-interview.com/x --resolve servicex.cb-interview.com:443:127.0.0.1 --header "Authorization: Bearer $token"
   ```
