# server.py
from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.middleware.cors import CORSMiddleware
import json

app = FastAPI()

# CORS 허용 (테스트용)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# 접속한 모든 클라이언트 저장 (player_id: websocket)
connected_clients: dict[str, WebSocket] = {}

@app.websocket("/ws/chat")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    player_id = None
    print("클라이언트 연결됨")

    try:
        while True:
            data = await websocket.receive_text()
            payload = json.loads(data)  # {"id": "player1", "x":1, "y":0, "z":2}
            player_id = payload["id"]

            # 연결 저장
            connected_clients[player_id] = websocket

            # 로그 출력
            print(f"{player_id} 로부터 받은 좌표: {payload}")

            # 브로드캐스트 (자기 자신 제외)
            # for pid, client in connected_clients.items():
            #     if pid != player_id:
            #         await client.send_text(json.dumps({
            #         "from": player_id,
            #         "x": payload["x"],
            #         "y": payload["y"],
            #         "z": payload["z"]
            #     }))
            for pid, client in connected_clients.items():

                await client.send_text(json.dumps({
                "from": player_id,
                "x": payload["x"],
                "y": payload["y"],
                "z": payload["z"]
            }))
    except WebSocketDisconnect:
        if player_id and player_id in connected_clients:
            del connected_clients[player_id]
        print(f"{player_id} 연결 종료")


################################################################################
# from fastapi import FastAPI, WebSocket
# from fastapi.middleware.cors import CORSMiddleware

# app = FastAPI()

# app.add_middleware(
#     CORSMiddleware,
#     allow_origins=["*"],  # 테스트용
#     allow_credentials=True,
#     allow_methods=["*"],
#     allow_headers=["*"],
# )

# @app.websocket("/ws/chat")
# async def websocket_endpoint(websocket: WebSocket):
#     await websocket.accept()
#     print("클라이언트 연결됨")

#     await websocket.send_text("안녕 난 서버")

#     while True:
#         try:
#             data = await websocket.receive_text()
#             print(f"Unity로부터 받은 메시지: {data}")
#             await websocket.send_text(f"서버가 받음: {data}")
#         except:
#             print("클라이언트 연결 종료")
#             break








################################################################################
# from fastapi import FastAPI, WebSocket
# from fastapi.middleware.cors import CORSMiddleware

# app = FastAPI()

# app.add_middleware(
#     CORSMiddleware,
#     allow_origins=["*"],  # 테스트용
#     allow_credentials=True,
#     allow_methods=["*"],
#     allow_headers=["*"],
# )

# @app.websocket("/ws/chat")
# async def websocket_endpoint(websocket: WebSocket):
#     await websocket.accept()
#     print("클라이언트 연결됨")

#     await websocket.send_text("안녕 난 서버")

#     while True:
#         try:
#             data = await websocket.receive_text()
#             print(f"Unity로부터 받은 메시지: {data}")
#             await websocket.send_text(f"서버가 받음: {data}")
#         except:
#             print("클라이언트 연결 종료")
#             break


################################################################################

