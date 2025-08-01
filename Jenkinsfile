pipeline {
    agent any

    environment {
        DOCKER_HUB_CREDENTIALS_ID = 'WebBanHangOnline'
        DOCKER_IMAGE_NAME = "springmuch/webbanhangonline" 
    }

    stages {
        stage('Configure Git') {
            steps {
                bat 'git config --global --add safe.directory "%WORKSPACE%"'
            }
        }

        stage('Restore Packages') {
            steps {
                echo 'Restoring .NET packages on agent...'
                bat 'dotnet restore WebBanHangOnline.sln'
            }
        }

        stage('Build Project') {
            steps {
                echo 'Building the project on agent...'
                bat 'dotnet build WebBanHangOnline.sln --configuration Release --no-restore'
            }
        }

        stage('Run Tests') {
            steps {
                echo 'Running tests...'
                bat 'dotnet test WebBanHangOnline.sln --no-build --verbosity normal'
            }
        }
        
        stage('Build and Push Docker Image') {
            steps {
                script {
                    def customImage = docker.build(DOCKER_IMAGE_NAME)

                    docker.withRegistry('https://registry.hub.docker.com', DOCKER_HUB_CREDENTIALS_ID) {
                        customImage.push("${env.BUILD_NUMBER}")
                        customImage.push("latest")
                    }
                }
            }
        }

        stage('Deploy to Kubernetes') {
            steps {
                // Giả sử bạn đã cấu hình Kubeconfig trong Jenkins
                bat 'kubectl apply -f secret.yml'
                bat 'kubectl apply -f db-deployment.yml'
                bat 'kubectl apply -f minio-deployment.yml'

                bat "kubectl set image deployment/webbanhang-app webbanhang-app=${DOCKER_IMAGE_NAME}:${env.BUILD_NUMBER}"
                bat 'kubectl apply -f app-deployment.yml'
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }
}