pipeline {
    agent any

    environment {
        DOCKER_HUB_CREDENTIALS_ID = 'WebBanHangOnline'
        DOCKER_IMAGE_NAME = "springmuch/webbanhangonline" 
	SOLUTION_NAME = "WebBanHangOnline.sln"
        PROJECT_NAME = "WebBanHangOnline.csproj"
    }

    stages {
        stage('Configure Git') {
            steps {
		echo "Adding Jenkins workspace to Git's safe directories..."
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
               bat "dotnet build ${SOLUTION_NAME} --configuration Release"
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
                    dir('.') {
                         def customImage = docker.build(DOCKER_IMAGE_NAME)

                         docker.withRegistry('https://registry.hub.docker.com', DOCKER_HUB_CREDENTIALS_ID) {
                             customImage.push("${env.BUILD_NUMBER}")
                             customImage.push("latest")
                         }
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