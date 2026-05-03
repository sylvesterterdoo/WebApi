pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Deploy with Docker Compose') {
            steps {
                sh 'docker compose down'
                sh 'docker compose up --build -d'
            }
        }

        stage('Verify Deployment') {
            steps {
                sleep 10
                sh 'docker ps'
                sh 'docker logs $(docker ps -1 --filter name=webapi)'
            }
        }
    }
}