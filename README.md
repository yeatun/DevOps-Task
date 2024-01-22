# WeatherAPI Application

## Overview

This repository contains the source code and configurations for the WeatherAPI application. The application is built using ASP.NET Core and is designed to retrieve weather information using the OpenWeatherMap API.And also buld for health check

## Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Build and Run Locally](#build-and-run-locally)
- [Dockerization](#dockerization)
  - [Dockerfile](#dockerfile)
  - [Build Docker Image](#build-docker-image)
  - [Push to Docker Hub](#push-to-docker-hub)
- [CI/CD Pipeline](#cicd-pipeline)
  - [Pipeline Configuration](#pipeline-configuration)
  - [Deployment to Kubernetes](#deployment-to-kubernetes)
- [Kubernetes Configuration](#kubernetes-configuration)
  - [Secrets](#secrets)
  - [Deployment YAML](#deployment-yaml)
- [Terraform and Ansible](#Terraform-Ansible)
## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)

### Build and Run Locally

1. Clone this repository:

    ```bash
    git clone https://github.com/your-username/weather-api.git
    cd weather-api
    ```

2. Build and run the application:

    ```bash
    dotnet build
    dotnet run
    ```

    The application will be accessible at `https://localhost:7275/swagger/index.html`.

## Dockerization

### Dockerfile

The Dockerfile in the project root directory is configured to create a lightweight Docker image for the WeatherAPI.

### Build Docker Image

Run the following commands to build the Docker image:

``` bash
docker build -t your-docker-username/weather-api:latest .
```
## CI/CD Pipeline

### Pipeline Configuration
To establish a connection between a Jenkins CI/CD server and the Kubernetes cluster, you need to specify the Kubernetes configuration file in the Jenkins server.

The Jenkins pipeline for this project is defined in the Jenkinsfile. Below is an explanation of each stage:

1. **Git Clone:**
   - Clones the project repository from GitHub.

2. **Restore Packages:**
   - Restores the NuGet packages required for the WeatherAPI.

3. **Build:**
   - Builds the WeatherAPI project.

4. **Pack:**
   - Publishes the WeatherAPI project for packaging.

5. **Docker Build:**
   - Builds a Docker image for the WeatherAPI using the provided Dockerfile.

6. **Docker Hub Login:**
   - Logs in to Docker Hub with credentials.

7. **Push Image to Docker Hub:**
   - Pushes the built Docker image to Docker Hub, tagged with the build number.

8. **SSH Into AMSL-Master node:**
   - SSH into the master node and perform the following steps:
     - Delete existing Kubernetes resources labeled with "app=weatherapi" in the "weather" namespace.
     - Copy the Kubernetes manifest file (weatherDeploy.yaml) to the master node.
     - Updates the image tag in the manifest file.
     - Deploys the WeatherAPI using kubectl apply.

## kubernetes-configuration
### Secrets
Create a Kubernetes Secret for sensitive information like API keys:
    ```bash
    kubectl create secret generic openweathermap-api-key --from-literal=api-key="YOUR_API_KEY_HERE"
    ```
## Terraform-Ansible

Automate Kubernetes cluster creation using on-premise servers via Digital Rebar, Terraform and Ansible

here use Digital Rebar, Terraform, and Ansible inside a virtual machine on Hyper-V to make it easier to set up on-premise machines (instead of on-premise machines  are going to use other virtual machines in Hyper-V). automating the complicated task of creating a Kubernetes cluster, so you can deploy it smoothly in your own on-site setup. This approach lets you manage your Kubernetes clusters more easily and accurately, all from within your virtual environment.
The choice to incorporate the Digital Rebar platform into  automation workflow for machine provisioning, alongside traditional tools like Terraform and cloud providers, stems from its unique capabilities tailored for on-premise environments. While Terraform and cloud providers excel in managing cloud resources, Digital Rebar specializes in orchestrating bare-metal and on-premise infrastructure, bridging the gap between software-defined infrastructure and physical hardware. It offers a robust set of features, including automated discovery, provisioning, and configuration management, making it a versatile solution for complex, heterogeneous on-premise setups. By integrating Digital Rebar into our automation strategy, we gain the flexibility to efficiently provision and manage on-premise machines, ensuring a seamless and adaptable approach to infrastructure automation.

### Prerequisites
-	Windows 10 Professional, Enterprise, or Education
-	PowerShell (Typically included with Windows 10)
-	PuTTY
-	Debian 10 Firmware ISO
-	ubuntu-20.04.3-live-server-amd64 iso image for kubernetes cluster (or any other linux OS iso you want for your cluster)
-	Hyper-V
-	RAM of your PC should be minimum 16GB (preferably more)

### Configuring Terraform in Control Machine

Terraform .tf files define infrastructure as code and work as follows:
1.	Resource Configuration: Declare desired infrastructure resources and their properties.
2.	Providers: Specify the infrastructure provider (e.g., AWS) to use.
3.	Variables: Use variables for configuration flexibility.
4.	Data Sources: Fetch data from existing resources.
5.	Output Values: Expose resource information.
6.	Modules: Organize code into reusable modules.
7.	State Management: Terraform maintains infrastructure state.
8.	Dependency Resolution: Terraform manages resource order.
9.	Execution: Use Terraform commands to apply changes.
10.	Change Management: Review and approve change plans.
11.	Idempotent: Terraform safely handles repeated runs.
12.	Version Control: Version control with tools like Git for code management.
Terraform simplifies infrastructure provisioning and management through code.

Steps to Install and configure Terraform for Digital rebar
1.	First we need to install terraform on the control machine
make a temporary directory:
mkdir ~/temp_keyring
cd ~/temp_keyring
2.	Download the GPG Key with curl:
Use curl to download the HashiCorp GPG key:
curl -o hashicorp.gpg https://apt.releases.hashicorp.com/gpg
3.	Import the GPG Key:
sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg hashicorp.gpg
4.	Update:
sudo apt update
5.	Install Terraform:
sudo apt-get install terraform
Now that we have installed terraform we will create the tf configuration file that will use Digital Rebar as the provider for resources.
The explanation of the terraform configuration file can be found here

6.	Create a directory for terraform:
mkdir terraform
cd terraform
7.	Now we create the configuration file:
sudo nano main.tf copy it

### Configuring Ansible playbooks to create the kubernetes cluster
need to install ansible
1.	Create a directory inside the terraform directory (This is very important for the automation)
command:
mkdir ansible
cd ansible
2.	Install Ansible
Commands:
sudo apt update
sudo apt install software-properties-common
sudo add-apt-repository --yes --update ppa:ansible/ansible
sudo apt install ansible
3.	Now we create the playbooks that will be executed from terraform to create the kubernetes cluster in the provisioned machine
First Playbook :
sudo nano install-k8s.yml
sudo nano master.yml
sudo nano join-workers.yml
### Run the automation
To run the automation process to create the kubernetes cluster We need to do the following steps:
-	Make sure the Control machine and the target machines are Turned on in Hyper-V
-	SSH using putty into the control machine
-	Go to the directory where terraform file is kept
-	Run this command:
export RS_ENDPOINT=https://<ip_address_of_your_endpoint>:8092/

export RS_KEY=rocketskates:<password >
Run terraform Commands:
- terraform init
- terraform plan
- terraform apply

### Explanation of the terraform configuration file

This Terraform configuration file appears to automate the deployment and configuration of infrastructure, specifically for setting up and managing a Kubernetes cluster using Digital Rebar Provision (DRP) as the infrastructure provider. Here's a breakdown of the key elements in this Terraform configuration:
#### Terraform Block:
This block specifies the required provider and its version. In this case, it's setting up the "drp" provider with a specific version and source from which to obtain the provider.
#### Provider Block:
The provider "drp" block configures the DRP provider. It specifies the endpoint URL for the DRP API server, which is used to manage the infrastructure.
#### Resource Block - drp_machine:
This resource block defines a set of DRP machines. It requests two machines from the "default" pool, indicating the desired number of machines and the pool to place them in.
#### Resource Block - time_sleep:
This resource is of type "time_sleep." It introduces a pause in the execution by waiting for a specified duration after the DRP machines have been created. It ensures that the machines are fully provisioned and ready before proceeding.
#### Locals Block:
This block defines a few local values for convenience. It stores the dependency on the drp_machine resource, as well as the IP addresses of the two DRP machines created earlier.
#### Output Blocks:
These blocks define output values, making information from the Terraform run available for reference. In this case, it exposes the instance IP addresses and individual IP addresses of the DRP machines.
#### Resource Block - null_resource (generate_hosts_file):
This resource is of type "null_resource" and is used for executing local commands or scripts as part of the provisioning process.
The depends_on attribute ensures that this resource only runs after the time_sleep resource has completed, giving the DRP machines time to become fully operational.
It contains multiple provisioner "local-exec" blocks, each specifying a different command to be executed on the local system:
The first block generates an Ansible hosts file with IP addresses for master and worker nodes.
The subsequent blocks execute Ansible commands to configure the Kubernetes cluster. They execute various Ansible playbooks to set up master and worker nodes.
In summary, this Terraform configuration automates the provisioning and configuration of a Kubernetes cluster using Digital Rebar Provision as the underlying infrastructure provider. It creates DRP machines, waits for them to be ready, and then uses Ansible to set up the Kubernetes cluster. The configuration defines dependencies and outputs for tracking and using information from the infrastructure deployment.









