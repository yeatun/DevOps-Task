terraform {
  required_providers {
    drp = {
      version = "2.1.12"
      source  = "rackn/drp"
    }
  }
}

provider "drp" {
  # Configuration options
  endpoint = "https://192.168.97.31:8092"
}

resource "drp_machine" "one_random_node" {
  pool = "default"
  count = 2
}

resource "time_sleep" "wait_120_seconds" {
  depends_on = [drp_machine.one_random_node]

  create_duration = "150s"
}

locals {
  depends_on = [drp_machine.one_random_node]
  instance_ips = drp_machine.one_random_node[*].address
  instance_ip1 = element(local.instance_ips, 0)
  instance_ip2 = element(local.instance_ips, 1)
}

output "instance_ips" {
  value = local.instance_ips
}

output "instance_ip1" {
  value = local.instance_ip1
}

output "instance_ip2" {
  value = local.instance_ip2
}

resource "null_resource" "generate_hosts_file" {
  depends_on = [time_sleep.wait_120_seconds]

  provisioner "local-exec" {
    command = <<-EOT
      echo "[masters]" > ./ansible/hosts
      echo "master ansible_host=${local.instance_ip1} ansible_user=root" >> ./ansible/hosts
      echo "[workers]" >> ./ansible/hosts
      echo "worker1 ansible_host=${local.instance_ip2} ansible_user=root" >> ./ansible/hosts
    EOT
  }
provisioner "local-exec" {
    command = "sudo sed -i 's/%sudo   ALL=(ALL:ALL) ALL/%sudo   ALL=(ALL:ALL) NOPASSWD: ALL/' /etc/sudoers"
  }
  provisioner "local-exec" {
    command = "cd ./ansible && ansible -i hosts all -m ping -e 'ansible_ssh_common_args=\"-o StrictHostKeyChecking=no\"'"
  }

  provisioner "local-exec" {
    command = "cd ./ansible && ansible-playbook -i hosts install-k8s.yml"
  }

  provisioner "local-exec" {
    command = "cd ./ansible && ansible-playbook -i hosts master.yml"
  }
  provisioner "local-exec" {
    command = "cd ./ansible && ansible-playbook -i hosts join-workers.yml"
  }
}
